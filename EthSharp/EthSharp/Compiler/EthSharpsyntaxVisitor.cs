using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EthSharp.ContractDevelopment;
using Microsoft.CodeAnalysis;
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

        // Massive TODO: Visit all syntax declarations

        public override void VisitMethodDeclaration(MethodDeclarationSyntax node)
        {
            // TODO: Do something with attributes 
            // TODO: Do something with parameterlist
            node.Body.Accept(this);
        }

        public override void VisitBlock(BlockSyntax node)
        {
            foreach (var statement in node.Statements)
            {
                statement.Accept(this);
            }
            if (Context.Assembly.Items.LastOrDefault()?.Instruction != EvmInstruction.RETURN)
                Context.Append(EvmInstruction.STOP); // Obviously this stops methods from being called internally. Need to work out correct pattern to handle this
        }

        public override void VisitExpressionStatement(ExpressionStatementSyntax node)
        {
            node.Expression.Accept(this);
        }

        public override void VisitAssignmentExpression(AssignmentExpressionSyntax node)
        {
            node.Right.Accept(this);

            //TODO: local variables
            var storageVar = (IdentifierNameSyntax) node.Left;
            Context.Append(Context.StorageIdentifiers[storageVar.Identifier.Text]);
            Context.Append(EvmInstruction.SSTORE);
        }

        public override void VisitReturnStatement(ReturnStatementSyntax node)
        {
            node.Expression.Accept(this);
            // need to MSTORE the value currently on top of stack
            Context.Append(UInt256.Zero);
            Context.Append(EvmInstruction.MSTORE);
            // then add values for where it is
            Context.Append(0x20);
            Context.Append(UInt256.Zero);
            Context.Append(EvmInstruction.RETURN);
        }

        public override void VisitBinaryExpression(BinaryExpressionSyntax node)
        {
            node.Left.Accept(this);
            node.Right.Accept(this);
            // TODO: Test these operators and add any missing
            switch (node.OperatorToken.Kind())
            {
                case SyntaxKind.PlusToken:
                    Context.Append(EvmInstruction.ADD);
                    break;
                case SyntaxKind.MinusToken:
                    Context.Append(EvmInstruction.SUB);
                    break;
                case SyntaxKind.AsteriskToken:
                    Context.Append(EvmInstruction.MUL);
                    break;
                case SyntaxKind.SlashToken:
                    Context.Append(EvmInstruction.DIV);
                    break;
                case SyntaxKind.PercentToken:
                    Context.Append(EvmInstruction.MOD);
                    break;
                default:
                    throw new NotImplementedException();
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
            switch (node.Kind())
            {
                case SyntaxKind.NumericLiteralExpression:
                    Context.Append((int) node.Token.Value);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        //TODO: Throw on all unready syntax until completed

        public override void Visit(SyntaxNode node)
        {
            throw new NotImplementedException();
        }

        public override void VisitConstructorDeclaration(ConstructorDeclarationSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitAccessorDeclaration(AccessorDeclarationSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitAccessorList(AccessorListSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitAliasQualifiedName(AliasQualifiedNameSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitAnonymousMethodExpression(AnonymousMethodExpressionSyntax node)
        {
            base.VisitAnonymousMethodExpression(node);
        }

        public override void VisitAnonymousObjectCreationExpression(AnonymousObjectCreationExpressionSyntax node)
        {
            base.VisitAnonymousObjectCreationExpression(node);
        }

        public override void VisitAnonymousObjectMemberDeclarator(AnonymousObjectMemberDeclaratorSyntax node)
        {
            base.VisitAnonymousObjectMemberDeclarator(node);
        }

        public override void VisitArgument(ArgumentSyntax node)
        {
            base.VisitArgument(node);
        }

        public override void VisitArgumentList(ArgumentListSyntax node)
        {
            base.VisitArgumentList(node);
        }

        public override void VisitArrayCreationExpression(ArrayCreationExpressionSyntax node)
        {
            base.VisitArrayCreationExpression(node);
        }

        public override void VisitArrayRankSpecifier(ArrayRankSpecifierSyntax node)
        {
            base.VisitArrayRankSpecifier(node);
        }

        public override void VisitArrayType(ArrayTypeSyntax node)
        {
            base.VisitArrayType(node);
        }

        public override void VisitArrowExpressionClause(ArrowExpressionClauseSyntax node)
        {
            base.VisitArrowExpressionClause(node);
        }

        public override void VisitAttribute(AttributeSyntax node)
        {
            base.VisitAttribute(node);
        }

        public override void VisitAttributeArgument(AttributeArgumentSyntax node)
        {
            base.VisitAttributeArgument(node);
        }

        public override void VisitAttributeArgumentList(AttributeArgumentListSyntax node)
        {
            base.VisitAttributeArgumentList(node);
        }

        public override void VisitAttributeList(AttributeListSyntax node)
        {
            base.VisitAttributeList(node);
        }

        public override void VisitAttributeTargetSpecifier(AttributeTargetSpecifierSyntax node)
        {
            base.VisitAttributeTargetSpecifier(node);
        }

        public override void VisitAwaitExpression(AwaitExpressionSyntax node)
        {
            base.VisitAwaitExpression(node);
        }

        public override void VisitBadDirectiveTrivia(BadDirectiveTriviaSyntax node)
        {
            base.VisitBadDirectiveTrivia(node);
        }

        public override void VisitBaseExpression(BaseExpressionSyntax node)
        {
            base.VisitBaseExpression(node);
        }

        public override void VisitBaseList(BaseListSyntax node)
        {
            base.VisitBaseList(node);
        }

        public override void VisitBracketedArgumentList(BracketedArgumentListSyntax node)
        {
            base.VisitBracketedArgumentList(node);
        }

        public override void VisitBracketedParameterList(BracketedParameterListSyntax node)
        {
            base.VisitBracketedParameterList(node);
        }

        public override void VisitBreakStatement(BreakStatementSyntax node)
        {
            base.VisitBreakStatement(node);
        }
    }
}
