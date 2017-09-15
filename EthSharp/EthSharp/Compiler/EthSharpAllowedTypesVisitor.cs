using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp;
using EthSharp.ContractDevelopment;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace EthSharp.Compiler
{
    public class EthSharpAllowedTypesVisitor : CSharpSyntaxWalker
    {
        private static HashSet<string> AllowedTypes = new HashSet<string>
        {
            typeof(UInt256).Name
        };

        private string ExceptionMessage => "Type not supported. Currently supported types: " + String.Join(", ", AllowedTypes);

        public EthSharpAllowedTypesVisitor()
        {            
        }

        public EthSharpAllowedTypesVisitor(HashSet<string> allowedTypes)
        {
            AllowedTypes = allowedTypes;
        }

        private bool TypeAllowed(string typeName)
        {
            return AllowedTypes.Contains(typeName);
        }

        private void CheckIfTypeIsAllowed(TypeSyntax type)
        {
            var typeName = type.GetText().ToString().Trim();

            if (!TypeAllowed(typeName))
            {
                throw new Exception("[" + typeName + "] " + ExceptionMessage); // TODO: Make these errors explicit
            }
        }
        
        public override void VisitFieldDeclaration(FieldDeclarationSyntax node)
        {
            CheckIfTypeIsAllowed(node.Declaration.Type);
        }

        public override void VisitPropertyDeclaration(PropertyDeclarationSyntax node)
        {
            CheckIfTypeIsAllowed(node.Type);
            base.VisitPropertyDeclaration(node);
        }

        public override void VisitParameter(ParameterSyntax node)
        {
            CheckIfTypeIsAllowed(node.Type);
        }

        public override void VisitMethodDeclaration(MethodDeclarationSyntax node)
        {
            CheckIfTypeIsAllowed(node.ReturnType);
            base.VisitMethodDeclaration(node);
        }

        public override void VisitLocalDeclarationStatement(LocalDeclarationStatementSyntax node)
        {
            CheckIfTypeIsAllowed(node.Declaration.Type);            
        }
    }
}
