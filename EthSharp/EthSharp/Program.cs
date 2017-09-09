using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EthSharp.Compiler;
using EthSharp.ContractDevelopment;
using HashLib;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace EthSharp
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                Console.Write(">>> ");
                ExecuteCommand(ParseCommand(Console.ReadLine()));
            }

            Console.ReadKey();
        }

        private static void CompileToCSharp(SyntaxTree tree)
        {
            var mscorlib = MetadataReference.CreateFromFile(typeof(object).Assembly.Location);
            var ethSharp = MetadataReference.CreateFromFile(typeof(UInt256).Assembly.Location);

            var options = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary);
            options = options.WithAllowUnsafe(true);                                //Allow unsafe code;
            options = options.WithOptimizationLevel(OptimizationLevel.Release);     //Set optimization level
            options = options.WithPlatform(Platform.X64);
            var compilation = CSharpCompilation.Create("MyCompilation",
                syntaxTrees: new[] { tree }, references: new[] { mscorlib, ethSharp },
                options: options);
            var emitResult = compilation.Emit("output.dll", "output.pdb");

            //If our compilation failed, we can discover exactly why.
            if (!emitResult.Success)
            {
                foreach (var diagnostic in emitResult.Diagnostics)
                {
                    Console.WriteLine(diagnostic.ToString());
                }
            }
        }

        private static void ExecuteCommand(string[] command)
        {
            switch (command[0])
            {
                case "compile":
                    string source = File.ReadAllText(command[1]);
                    var tree = SyntaxFactory.ParseSyntaxTree(source);
                    var evmByteCode = new EthSharpCompiler(tree).CreateByteCode();
                    Console.WriteLine(evmByteCode.ByteCode.ToHexString());
                    break;
                case "exit":
                    Environment.Exit(0);
                    break;
                default:
                    Console.WriteLine("Command '" + command[0] + "' unknown.");
                    break;
            }
        }

        private static string[] ParseCommand(string command)
        {
            return command.Split(' ');
        }
    }
}
