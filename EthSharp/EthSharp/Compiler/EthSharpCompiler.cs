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
        private SyntaxTree Root { get; set; }

        private ClassDeclarationSyntax RootClass => Root.GetRoot().ChildNodes().OfType<ClassDeclarationSyntax>().FirstOrDefault();

        public EthSharpCompiler(SyntaxTree root)
        {
            Context = new EthSharpCompilerContext();
            SyntaxVisitor = new EthSharpSyntaxVisitor(Context);
            Root = root;
        }

        public EvmByteCode CreateByteCode()
        {
            // for now just assume one class
            InitializeContext();
            Dictionary<byte[], PropertyDeclarationSyntax> propertyGetters = RootClass.GetProperties().ToDictionary(x => x.GetGetterAbiSignature(), x => x);
            Dictionary<byte[], MethodDeclarationSyntax> methods = RootClass.GetMethods().ToDictionary(x => x.GetAbiSignature(), x => x);
            Dictionary<byte[], EthSharpAssemblyItem> methodEntryPoints = new Dictionary<byte[], EthSharpAssemblyItem>();
            RetrieveFunctionHash();

            // If incoming call data matches get, jump to that function
            foreach (var property in propertyGetters)
            {
                methodEntryPoints.Add(property.Key, new EthSharpAssemblyItem(AssemblyItemType.Tag, Context.GetNewTag()));
                Context.Append(EvmInstruction.DUP1);
                Context.Append(new UInt256(property.Key));
                Context.Append(EvmInstruction.EQ);
                Context.Append(new EthSharpAssemblyItem(AssemblyItemType.PushTag, methodEntryPoints[property.Key].Data)); // this should be cleaner
                Context.Append(EvmInstruction.JUMPI);
            }

            // If incoming call data matches function, jump to that function.
            foreach (var method in methods)
            {
                methodEntryPoints.Add(method.Key, new EthSharpAssemblyItem(AssemblyItemType.Tag,Context.GetNewTag()));
                Context.Append(EvmInstruction.DUP1);
                Context.Append(new UInt256(method.Key));
                Context.Append(EvmInstruction.EQ);
                Context.Append(new EthSharpAssemblyItem(AssemblyItemType.PushTag,methodEntryPoints[method.Key].Data)); // this should be cleaner
                Context.Append(EvmInstruction.JUMPI);
            }
            // bottom of function switch - fail
            Context.Append(EvmInstruction.INVALID);

            foreach (var property in propertyGetters)
            {
                Context.Append(methodEntryPoints[property.Key]);
                Context.Append(Context.StorageIdentifiers[property.Value.Identifier.Text]); //push data location
                Context.Append(EvmInstruction.SLOAD); //sload it
                Context.Append(EvmInstruction.RETURN);//return
            }

            foreach (var method in methods)
            {
                Context.Append(methodEntryPoints[method.Key]); // sets destination to jump to from switch
                // would implement payable and other attributes here
                // get calldata if necessary
                method.Value.Accept(SyntaxVisitor); //This should do all the magic!
            }

            var contractByteCode = Context.Assembly.Assemble();
            return WrapByteCode(contractByteCode);
        }

        private EvmByteCode WrapByteCode(EvmByteCode toWrap)
        {
            throw new NotImplementedException();
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
            foreach (var localVariable in RootClass.GetProperties())
            {
                Context.AssignNewStorageLocation(localVariable.Identifier.Text);
            }
        }
    }
}
