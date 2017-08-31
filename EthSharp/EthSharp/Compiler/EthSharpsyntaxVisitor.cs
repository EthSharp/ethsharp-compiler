using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace EthSharp.Compiler
{
    public class EthSharpSyntaxVisitor : CSharpSyntaxVisitor
    {
        private EthSharpCompilerContext Context { get; set; }

        public EthSharpSyntaxVisitor(EthSharpCompilerContext context)
        {
            Context = context;
        }

        public override void VisitMethodDeclaration(MethodDeclarationSyntax node)
        {
            //Should do something with attributes 

            //Something with parameterlist
            
            // Compile actual block
            node.Body.Accept(this);
        }

        public override void VisitBlock(BlockSyntax node)
        {
            foreach (var statement in node.Statements)
            {
                statement.Accept(this);
            }
        }

        public override void VisitReturnStatement(ReturnStatementSyntax node)
        {
            node.Expression.Accept(this);
            Context.Append(EvmInstruction.RETURN);
        }

        public override void VisitBinaryExpression(BinaryExpressionSyntax node)
        {
            node.Left.Accept(this);
            node.Right.Accept(this);

            // There is probably a way to handle this by type?

            switch (node.OperatorToken.Text)
            {
                case "+":
                    Context.Append(EvmInstruction.ADD);
                    break;
            }
        }

        public override void VisitIdentifierName(IdentifierNameSyntax node)
        {
            if (Context.StorageIdentifiers.ContainsKey(node.Identifier.Text))
            {
                Context.Append(Context.StorageIdentifiers[node.Identifier.Text]); // push memory location onto stack
                Context.Append(EvmInstruction.SLOAD);
            }
        }

        public override void VisitLiteralExpression(LiteralExpressionSyntax node)
        {
            base.VisitLiteralExpression(node);
        }
    }
}
