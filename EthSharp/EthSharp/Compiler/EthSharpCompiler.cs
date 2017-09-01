using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Security.Cryptography;
using HashLib;
using EthSharp.ContractDevelopment;

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

        public EthSharpAssembly Create()
        {
            // for now just assume one class
            InitializeContext();
            Dictionary<byte[], MethodDeclarationSyntax> methods = RootClass.GetMethods().ToDictionary(x => x.GetAbiSignature(), x => x);
            Dictionary<byte[], EthSharpAssemblyItem> methodEntryPoints = new Dictionary<byte[], EthSharpAssemblyItem>();
            RetrieveFunctionHash();

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

            foreach (var method in methods)
            {
                Context.Append(methodEntryPoints[method.Key]); // sets destination to jump to from switch
                // would implement payable and other attributes here
                // get calldata if necessary
                method.Value.Accept(SyntaxVisitor); //This should do all the magic!
            }

            var test = Context.Assembly.Assemble();
            throw new NotImplementedException();
        }

        /// <summary>
        /// Don't yet understand the random hardcoded bytes but retrieves function hash from transaction data
        /// </summary>
        private void RetrieveFunctionHash()
        {
            Context.Append(UInt256.Zero);
            Context.Append(EvmInstruction.CALLDATALOAD);
            Context.Append(UInt256.FromByteArrayBE(new byte[]
            {
                0,0,0,1,0,0,0,0,
                0,0,0,0,0,0,0,0,
                0,0,0,0,0,0,0,0,
                0,0,0,0,0,0,0,0
            })); //what are these values?
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
            //Taken from solidity wrapper - still trying to understand.
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
