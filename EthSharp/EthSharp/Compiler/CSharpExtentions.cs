using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EthSharp.Hashing;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace EthSharp.Compiler
{
    public static class CSharpExtentions
    {
        //Need to implement get function hash for mapping!
        public static IList<MethodDeclarationSyntax> GetMethods(this ClassDeclarationSyntax classDeclarationSyntax)
        {
            return classDeclarationSyntax.Members.OfType<MethodDeclarationSyntax>().ToList();
        }

        public static string GetExternalSignature(this MethodDeclarationSyntax method)
        {
            string toReturn = method.Identifier.Text;
            toReturn += "(";
            for (int i = 0; i < method.ParameterList.Parameters.Count; i++)
            {
                toReturn += method.ParameterList.Parameters[i].Type.ToString();

                if (i != method.ParameterList.Parameters.Count - 1)
                    toReturn += ",";
            }
            return toReturn + ")";
        }

        public static byte[] GetAbiSignature(this MethodDeclarationSyntax method)
        {
            return HashHelper.Keccak256(method.GetExternalSignature()).GetBytes().Take(4).ToArray();
        }
    }
}
