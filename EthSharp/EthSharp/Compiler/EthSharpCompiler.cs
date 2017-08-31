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


        public EthSharpCompiler()
        {
            Context = new EthSharpCompilerContext();
        }

        public EthSharpAssembly Create(SyntaxTree root)
        {
            // for now just assume one class
            var rootClass = root.GetRoot().ChildNodes().OfType<ClassDeclarationSyntax>().FirstOrDefault(); // cast as class type
            InitializeContext();
            var classDeclarationSyntax = rootClass;
            Dictionary<byte[], MethodDeclarationSyntax> methods = classDeclarationSyntax.GetMethods().ToDictionary(x => x.GetAbiSignature(), x => x);
            Dictionary<byte[], EthSharpAssemblyItem> methodEntryPoints = new Dictionary<byte[], EthSharpAssemblyItem>();
            RetrieveFunctionHash();

            // If incoming call data matches function, jump to that function.
            foreach (var method in methods)
            {
                methodEntryPoints.Add(method.Key, new EthSharpAssemblyItem(AssemblyItemType.PushTag,Context.GetNewTag()));
                Context.Append(EvmInstruction.DUP1);
                Context.Append(new UInt256(method.Key));
                Context.Append(EvmInstruction.EQ);
                Context.Append(methodEntryPoints[method.Key]);
                Context.Append(EvmInstruction.JUMPI);
            }

            // if we reached here (bottom of the function list) - no function hashes were matched, so fail.
            // if fallback was to be implemented, it would happen here.
            Context.Append(EvmInstruction.INVALID);

            foreach (var method in methods)
            {
                Context.Append(methodEntryPoints[method.Key]); //Adds the JUMPDEST
                //Get calldata if necessary
                // add SLOAD
                // POP
                //If we want a 'payable' modifier or attribute, we would set it here- just a callvalue check == 0



                //build actual method logic 
                //end by appending Instruction.Stop or Instruction.Return;
            }

            var test = Context.Assembly.Assemble();
            throw new NotImplementedException();
        }



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

        //Would probably rather not even use now whilst I don't understand what this does - I think it's the
        //packaging that stops the bytecode being removed after use but I'm not sure
        private void InitializeContext()
        {
            //need to use uint here eventually. 

            //also need to work out why we do this to begin with

            //can use freeMemoryPointer const here

            // Compiled Contracts go here in solidity

            Context.Append(64 + 32);
            Context.Append(64);
            Context.Append(EvmInstruction.MSTORE);

            // Registering state variables goes here in solidity. Not sure if important here

            // Also resets visiting of node. Unsure how important
        }
    }
}
