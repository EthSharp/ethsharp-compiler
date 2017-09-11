﻿using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis.CSharp;
using EthSharp.ContractDevelopment;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace EthSharp.Compiler
{
    public class EthSharpAllowedTypesVisitor : CSharpSyntaxVisitor
    {
        private static readonly Dictionary<string, bool> AllowedTypes = new Dictionary<string, bool>
        {
            {typeof(UInt256).Name, true}
        };

        private string ExceptionMessage => "Type not supported. Currently supported types: " + String.Join(", ", AllowedTypes.Keys);

        private bool TypeAllowed(TypeSyntax type)
        {
            return AllowedTypes.ContainsKey(type.GetText().ToString().Trim());
        }


        public override void VisitClassDeclaration(ClassDeclarationSyntax node)
        {
            foreach (var property in node.GetProperties())
            {
                if (!TypeAllowed(property.Type))
                    throw new NotImplementedException(ExceptionMessage); // TODO: Make these errors explicit
            }
            foreach (var field in node.GetFields())
            {
                if (!TypeAllowed(field.Declaration.Type))
                    throw new NotImplementedException(ExceptionMessage);
            }
            foreach (var method in node.GetMethods())
            {
                method.Accept(this);
            }
        }

        public override void VisitMethodDeclaration(MethodDeclarationSyntax node)
        {
            node.ParameterList.Accept(this);
            node.Body.Accept(this);
        }

        public override void VisitParameterList(ParameterListSyntax node)
        {
            foreach (var parameter in node.Parameters)
            {
                if (!TypeAllowed(parameter.Type))
                    throw new NotImplementedException(ExceptionMessage);
            }
        }

        public override void VisitBlock(BlockSyntax node)
        {
            foreach (var statement in node.Statements)
            {
                statement.Accept(this);
            }
        }
    }
}
