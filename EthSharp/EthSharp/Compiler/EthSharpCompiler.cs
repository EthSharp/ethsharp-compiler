using System;
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
            //for now just assume one class
            var rootClass = root.GetRoot().ChildNodes().FirstOrDefault(); // cast as class type
            InitializeContext();
            var classDeclarationSyntax = (ClassDeclarationSyntax) rootClass;

            //create a dictionary of some hash from functionname so that functions can be called just like solidity


            //asuming that we have any functions, create the evm code to read the function as sent...

            foreach (var method in classDeclarationSyntax.GetMethods())
            {
                //string test = method.GetExternalSignature();
                //it seems like here we compose some kind of conditional that decides which command to execute. 

                //if it matches one, it sends the request to the required code 
            }

            //add invalid instruction - no fallbacks for now

            foreach (var method in classDeclarationSyntax.Members.Where(x => x is MethodDeclarationSyntax))
            {
                //here, build addresses and actual logic - use go to from above to get here

                //will always end by appending Instruction.Stop or Instruction.Return
            }


            throw new NotImplementedException();
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
