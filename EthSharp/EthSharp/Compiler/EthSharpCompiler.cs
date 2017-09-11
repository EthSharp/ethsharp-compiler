using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Security.Cryptography;
using HashLib;
using EthSharp.ContractDevelopment;
using Microsoft.CodeAnalysis.CSharp;

namespace EthSharp.Compiler
{
    public class EthSharpCompiler
    {
        private EthSharpCompilerContext Context { get; set; }

        private EthSharpSyntaxVisitor SyntaxVisitor { get; set; }

        public EthSharpCompiler(SyntaxTree root)
        {
            Context = new EthSharpCompilerContext(root);
            SyntaxVisitor = new EthSharpSyntaxVisitor(Context);
        }

        public EvmByteCode CreateByteCode()
        {
            // for now just assume one class
            InitializeContext();

            Context.RootClass.Accept(new EthSharpAllowedTypesVisitor()); // parse class and throw exception if any unexpected types used

            Dictionary<byte[], PropertyDeclarationSyntax> propertyGetters = Context.RootClass.GetProperties().ToDictionary(x => x.GetGetterAbiSignature(), x => x);
            Dictionary<byte[], MethodDeclarationSyntax> methods = Context.RootClass.GetPublicMethods().ToDictionary(x => x.GetAbiSignature(), x => x);
            Dictionary<byte[], EthSharpAssemblyItem> publicMethodEntryPoints = new Dictionary<byte[], EthSharpAssemblyItem>();
            RetrieveFunctionHash();

            // If incoming call data matches get, jump to that function - TODO: Only public
            foreach (var property in propertyGetters)
            {
                publicMethodEntryPoints.Add(property.Key, new EthSharpAssemblyItem(AssemblyItemType.Tag, Context.GetNewTag()));
                Context.Append(EvmInstruction.DUP1);
                Context.Append(new UInt256(property.Key));
                Context.Append(EvmInstruction.EQ);
                Context.Append(new EthSharpAssemblyItem(AssemblyItemType.PushTag, publicMethodEntryPoints[property.Key].Data)); // this should be cleaner
                Context.Append(EvmInstruction.JUMPI);
            }

            // If incoming call data matches function, jump to that function - only public
            foreach (var method in methods.Where(x=>x.Value.Modifiers.Any(y => y.Kind() == SyntaxKind.PublicKeyword)))
            {
                publicMethodEntryPoints.Add(method.Key, new EthSharpAssemblyItem(AssemblyItemType.Tag,Context.GetNewTag()));
                Context.Append(EvmInstruction.DUP1);
                Context.Append(new UInt256(method.Key));
                Context.Append(EvmInstruction.EQ);
                Context.Append(new EthSharpAssemblyItem(AssemblyItemType.PushTag, publicMethodEntryPoints[method.Key].Data)); // this should be cleaner
                Context.Append(EvmInstruction.JUMPI);
            }
            // bottom of function switch - fail
            Context.Append(EvmInstruction.INVALID);

            foreach (var property in propertyGetters)
            {
                Context.Append(publicMethodEntryPoints[property.Key]);
                Context.Append(Context.StorageIdentifiers[property.Value.Identifier.Text]); //push data location
                Context.Append(EvmInstruction.SLOAD); //sload it
                Context.Append(EvmInstruction.RETURN);//return
            }

            //  actual method compilation happens in here. Lets treat each public function like an entry point.
            foreach (var method in methods)
            {
                CompilePublicMethod(method.Value, publicMethodEntryPoints[method.Key]); //This should do all the magic!
            }

            CompileMethodQueue();

            var contractByteCode = Context.Assembly.Assemble();
            return WrapByteCode(contractByteCode);
        }

        private void CompilePublicMethod(MethodDeclarationSyntax method, EthSharpAssemblyItem jumpDest)
        {
            Context.Append(jumpDest); // jumpdest for this public method
            int tagToJumpOutTo = Context.GetNewTag();
            Context.Append(new EthSharpAssemblyItem(AssemblyItemType.PushTag, tagToJumpOutTo)); // push jump back tag
            var internalMethodTag = Context.GetMethodTag(method);
            Context.Append(new EthSharpAssemblyItem(AssemblyItemType.PushTag, internalMethodTag));  // push private jump tag
            Context.Append(EvmInstruction.JUMP);
            Context.Append(new EthSharpAssemblyItem(AssemblyItemType.Tag, tagToJumpOutTo)); // jump back tag
            if (method.ReturnType.Kind() == SyntaxKind.VoidKeyword)
            {
                Context.Append(EvmInstruction.STOP);
            }
            else
            {
                // need to MSTORE the value currently on top of stack
                Context.Append(UInt256.Zero);
                Context.Append(EvmInstruction.MSTORE);
                // then add values for where it is
                Context.Append(0x20);
                Context.Append(UInt256.Zero);
                Context.Append(EvmInstruction.RETURN);
            }
        }

        private void CompileMethodQueue()
        {
            while (Context.MethodQueue.Any())
            {
                var methodToCompile = Context.MethodQueue.Dequeue();
                methodToCompile.Accept(SyntaxVisitor);
            }
        }

        // constructor also goes here
        private EvmByteCode WrapByteCode(EvmByteCode toWrap)
        {
            var preAssembly = new EthSharpAssembly();
            preAssembly.Append(toWrap.ByteCode.Count); // push length of bytecode
            preAssembly.Append(0x0c); // push offset of bytecode - TODO: THIS COULD CHANGE IF THIS BLOCK CHANGES!
            preAssembly.Append(UInt256.Zero); // push memory location to store in (0)
            preAssembly.Append(EvmInstruction.CODECOPY); // codecopy
            preAssembly.Append(toWrap.ByteCode.Count); // push length of code to pick
            preAssembly.Append(UInt256.Zero); // push location of code (0)
            preAssembly.Append(EvmInstruction.RETURN);
            var preByteCode = preAssembly.Assemble();
            toWrap.ByteCode.InsertRange(0, preByteCode.ByteCode);
            return toWrap;
        }

        /// <summary>
        /// Retrieves function hash from transaction data
        /// </summary>
        private void RetrieveFunctionHash()
        {
            Context.Append(UInt256.Zero); // offset for calldata
            Context.Append(EvmInstruction.CALLDATALOAD);
            Context.Append(UInt256.FromByteArrayBE(new byte[]
            {
                0,0,0,1,0,0,0,0,
                0,0,0,0,0,0,0,0,
                0,0,0,0,0,0,0,0,
                0,0,0,0,0,0,0,0
            })); //hardcoded bytes used to get first 4 bytes of transaction data
            Context.Append(EvmInstruction.SWAP1);
            Context.Append(EvmInstruction.DIV);
            Context.Append(UInt256.FromByteArrayBE(new byte[]
            {
                0,0,0,0,0,0,0,0,
                0,0,0,0,0,0,0,0,
                0,0,0,0,0,0,0,0,
                0,0,0,0,255,255,255,255
            })); //and here
            Context.Append(EvmInstruction.AND);
        }

        private void InitializeContext()
        {
            //Taken from solidity wrapper - added to front of contract
            Context.Append(64 + 32);
            Context.Append(64);
            Context.Append(EvmInstruction.MSTORE);

            //TODO: Include fieldDeclarations too 
            foreach (var localVariable in Context.RootClass.GetProperties())
            {
                Context.AssignNewStorageLocation(localVariable.Identifier.Text);
            }
        }
    }
}
