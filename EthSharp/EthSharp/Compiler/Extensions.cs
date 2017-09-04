using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EthSharp.Hashing;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace EthSharp.Compiler
{
    public static class Extensions
    {
        public static IList<MethodDeclarationSyntax> GetMethods(this ClassDeclarationSyntax classDeclarationSyntax)
        {
            return classDeclarationSyntax.Members.OfType<MethodDeclarationSyntax>().ToList();
        }

        public static IList<PropertyDeclarationSyntax> GetProperties(this ClassDeclarationSyntax classDeclarationSyntax)
        {
            return classDeclarationSyntax.Members.OfType<PropertyDeclarationSyntax>().ToList();
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
            toReturn += ")";

            //apply transformations to match ABI
            return ApplyAbiTransformations(toReturn);
        }

        //Reverse is cos of big endian
        public static byte[] GetAbiSignature(this MethodDeclarationSyntax method)
        {
            return HashHelper.Keccak256(method.GetExternalSignature()).GetBytes().Take(4).Reverse().ToArray();
        }

        public static byte[] GetGetterAbiSignature(this PropertyDeclarationSyntax property)
        {
            string signature = property.Identifier.Text + "()";
            return HashHelper.Keccak256(signature).GetBytes().Take(4).Reverse().ToArray();
        }

        public static byte[] GetAbiSignatureSetter(this PropertyDeclarationSyntax method)
        {
            throw new NotImplementedException();
        }

        public static string ToHexString(this IEnumerable<byte> ba)
        {
            string hex = BitConverter.ToString(ba.ToArray());
            return hex.Replace("-", "");
        }

        private static string ApplyAbiTransformations(string input)
        {
            return input.Replace("UInt256", "uint256");
        }
    }
}
