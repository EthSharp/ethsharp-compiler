using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EthSharp.Compiler;
using Microsoft.CodeAnalysis.CSharp;

namespace EthSharp
{
    class Program
    {
        static void Main(string[] args)
        {
            string source = @"public class SimpleStorage {
    private int storedData;

    public void Set(int x) {
        storedData = x;
    }

    public int Get(){
        return storedData;
    }
}";
            //parse tree - create assembly then emit.

            var tree = SyntaxFactory.ParseSyntaxTree(source);
            var assembly = new EthSharpCompiler().Create(tree);
            Console.ReadKey();
        }
    }
}
