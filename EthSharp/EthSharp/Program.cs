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
            Array.Reverse(args); // Stack reads in values backwards
            Stack<string> arguments = new Stack<string>(args);
            ArgSettings settings = new ArgSettings();
            
            while(arguments.Count != 0)
            {
                switch (arguments.Pop())
                {
                    case "-i": // Define input file
                    case "--input":
                        settings.inputFile = arguments.Pop();
                        break;
                    case "-o": // Define output file. If null, just print output to console.
                    case "--output":
                        settings.outputFile = arguments.Pop();
                        break;
                    case "-h": // Display help info
                    case "--help":
                    case "?":
                        DisplayHelpInfo();
                        break;
                }
            }

            string source;
            if (settings.inputFile != null)
            {
                source = File.ReadAllText(settings.inputFile);
                var tree = SyntaxFactory.ParseSyntaxTree(source);
                var evmByteCode = new EthSharpCompiler(tree).CreateByteCode();
                if(settings.outputFile != null)
                {
                    File.WriteAllText(settings.outputFile, evmByteCode.ByteCode.ToHexString());
                }
                else
                {
                    Console.WriteLine(evmByteCode.ByteCode.ToHexString());
                }
                Console.ReadKey();
            }
            else
            {
                Console.WriteLine("No input file defined.");
                Environment.Exit(1);
            }
        }

        private static void DisplayHelpInfo()
        {
            throw new NotImplementedException();
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
    }

    struct ArgSettings
    {
        public string inputFile;
        public string outputFile;
    }
}
