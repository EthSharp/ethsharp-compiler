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

        public override void VisitMethodDeclaration(MethodDeclarationSyntax node)
        {
            // TODO: Do something with attributes 
            // TODO: Do something with parameterlist
            Context.Append(Context.MethodBlockEntryPoints[node.Identifier.Text]);
            node.Body.Accept(this);
        }

        public override void VisitBlock(BlockSyntax node)
        {
            foreach (var statement in node.Statements)
            {
                statement.Accept(this);
            }
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
            Context.Append(EvmInstruction.SWAP1); // swaps so that place to jump out to is in right place
            Context.Append(EvmInstruction.JUMP); // jump out of current method - assumes that place to jump to was on stack at start
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
                    Context.Append((int) node.Token.Value); //handling UInt256
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        // A method() call. Ignoring parameters for now
        public override void VisitInvocationExpression(InvocationExpressionSyntax node)
        {
            // push tag to jump out to
            int tagToJumpOutTo = Context.GetNewTag();
            Context.Append(new EthSharpAssemblyItem(AssemblyItemType.PushTag, tagToJumpOutTo));
            // push tag to jump in to
            string methodName = ((IdentifierNameSyntax) node.Expression).Identifier.Text;
            var method = Context.RootClass.GetMethods().FirstOrDefault(x => x.Identifier.Text == methodName);
            var tag = Context.GetMethodTag(method); // tag with ID to jump to
            Context.Append(new EthSharpAssemblyItem(AssemblyItemType.PushTag, tag));
            // enter method
            Context.Append(EvmInstruction.JUMP);
            // after method runs, add jumpdest to exit function.
            Context.Append(new EthSharpAssemblyItem(AssemblyItemType.Tag, tagToJumpOutTo));
        }

        #region unimplemented

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
            throw new NotImplementedException();
        }

        public override void VisitAnonymousObjectCreationExpression(AnonymousObjectCreationExpressionSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitAnonymousObjectMemberDeclarator(AnonymousObjectMemberDeclaratorSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitArgument(ArgumentSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitArgumentList(ArgumentListSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitArrayCreationExpression(ArrayCreationExpressionSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitArrayRankSpecifier(ArrayRankSpecifierSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitArrayType(ArrayTypeSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitArrowExpressionClause(ArrowExpressionClauseSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitAttribute(AttributeSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitAttributeArgument(AttributeArgumentSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitAttributeArgumentList(AttributeArgumentListSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitAttributeList(AttributeListSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitAttributeTargetSpecifier(AttributeTargetSpecifierSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitAwaitExpression(AwaitExpressionSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitBadDirectiveTrivia(BadDirectiveTriviaSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitBaseExpression(BaseExpressionSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitBaseList(BaseListSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitBracketedArgumentList(BracketedArgumentListSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitBracketedParameterList(BracketedParameterListSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitBreakStatement(BreakStatementSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitCasePatternSwitchLabel(CasePatternSwitchLabelSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitCaseSwitchLabel(CaseSwitchLabelSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitCastExpression(CastExpressionSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitCatchClause(CatchClauseSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitCatchDeclaration(CatchDeclarationSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitCatchFilterClause(CatchFilterClauseSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitCheckedExpression(CheckedExpressionSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitCheckedStatement(CheckedStatementSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitClassDeclaration(ClassDeclarationSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitClassOrStructConstraint(ClassOrStructConstraintSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitCompilationUnit(CompilationUnitSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitConditionalAccessExpression(ConditionalAccessExpressionSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitConditionalExpression(ConditionalExpressionSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitConstantPattern(ConstantPatternSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitConstructorConstraint(ConstructorConstraintSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitConstructorInitializer(ConstructorInitializerSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitContinueStatement(ContinueStatementSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitConversionOperatorDeclaration(ConversionOperatorDeclarationSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitConversionOperatorMemberCref(ConversionOperatorMemberCrefSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitCrefBracketedParameterList(CrefBracketedParameterListSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitCrefParameter(CrefParameterSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitCrefParameterList(CrefParameterListSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitDeclarationExpression(DeclarationExpressionSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitDeclarationPattern(DeclarationPatternSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitDefaultExpression(DefaultExpressionSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitDefaultSwitchLabel(DefaultSwitchLabelSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitDefineDirectiveTrivia(DefineDirectiveTriviaSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitDelegateDeclaration(DelegateDeclarationSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitDestructorDeclaration(DestructorDeclarationSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitDiscardDesignation(DiscardDesignationSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitDoStatement(DoStatementSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitDocumentationCommentTrivia(DocumentationCommentTriviaSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitElementAccessExpression(ElementAccessExpressionSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitElementBindingExpression(ElementBindingExpressionSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitElifDirectiveTrivia(ElifDirectiveTriviaSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitElseClause(ElseClauseSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitElseDirectiveTrivia(ElseDirectiveTriviaSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitEmptyStatement(EmptyStatementSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitEndIfDirectiveTrivia(EndIfDirectiveTriviaSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitEndRegionDirectiveTrivia(EndRegionDirectiveTriviaSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitEnumDeclaration(EnumDeclarationSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitEnumMemberDeclaration(EnumMemberDeclarationSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitEqualsValueClause(EqualsValueClauseSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitErrorDirectiveTrivia(ErrorDirectiveTriviaSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitEventDeclaration(EventDeclarationSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitEventFieldDeclaration(EventFieldDeclarationSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitExplicitInterfaceSpecifier(ExplicitInterfaceSpecifierSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitExternAliasDirective(ExternAliasDirectiveSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitFieldDeclaration(FieldDeclarationSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitFinallyClause(FinallyClauseSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitFixedStatement(FixedStatementSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitForEachStatement(ForEachStatementSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitForEachVariableStatement(ForEachVariableStatementSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitForStatement(ForStatementSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitFromClause(FromClauseSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitGenericName(GenericNameSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitGlobalStatement(GlobalStatementSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitGotoStatement(GotoStatementSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitGroupClause(GroupClauseSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitImplicitArrayCreationExpression(ImplicitArrayCreationExpressionSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitIfDirectiveTrivia(IfDirectiveTriviaSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitIfStatement(IfStatementSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitImplicitElementAccess(ImplicitElementAccessSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitIncompleteMember(IncompleteMemberSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitIndexerDeclaration(IndexerDeclarationSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitIndexerMemberCref(IndexerMemberCrefSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitInitializerExpression(InitializerExpressionSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitInterfaceDeclaration(InterfaceDeclarationSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitInterpolatedStringExpression(InterpolatedStringExpressionSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitInterpolatedStringText(InterpolatedStringTextSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitInterpolation(InterpolationSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitInterpolationAlignmentClause(InterpolationAlignmentClauseSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitInterpolationFormatClause(InterpolationFormatClauseSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitIsPatternExpression(IsPatternExpressionSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitJoinClause(JoinClauseSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitJoinIntoClause(JoinIntoClauseSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitLabeledStatement(LabeledStatementSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitLetClause(LetClauseSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitLineDirectiveTrivia(LineDirectiveTriviaSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitLoadDirectiveTrivia(LoadDirectiveTriviaSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitLocalDeclarationStatement(LocalDeclarationStatementSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitLocalFunctionStatement(LocalFunctionStatementSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitLockStatement(LockStatementSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitMakeRefExpression(MakeRefExpressionSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitMemberAccessExpression(MemberAccessExpressionSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitMemberBindingExpression(MemberBindingExpressionSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitNameColon(NameColonSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitNameEquals(NameEqualsSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitNameMemberCref(NameMemberCrefSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitNamespaceDeclaration(NamespaceDeclarationSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitNullableType(NullableTypeSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitObjectCreationExpression(ObjectCreationExpressionSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitOmittedArraySizeExpression(OmittedArraySizeExpressionSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitOmittedTypeArgument(OmittedTypeArgumentSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitOperatorDeclaration(OperatorDeclarationSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitOperatorMemberCref(OperatorMemberCrefSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitOrderByClause(OrderByClauseSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitOrdering(OrderingSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitParameter(ParameterSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitParameterList(ParameterListSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitParenthesizedExpression(ParenthesizedExpressionSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitParenthesizedLambdaExpression(ParenthesizedLambdaExpressionSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitParenthesizedVariableDesignation(ParenthesizedVariableDesignationSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitPointerType(PointerTypeSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitPostfixUnaryExpression(PostfixUnaryExpressionSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitPragmaChecksumDirectiveTrivia(PragmaChecksumDirectiveTriviaSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitPragmaWarningDirectiveTrivia(PragmaWarningDirectiveTriviaSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitPredefinedType(PredefinedTypeSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitPrefixUnaryExpression(PrefixUnaryExpressionSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitPropertyDeclaration(PropertyDeclarationSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitQualifiedCref(QualifiedCrefSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitQualifiedName(QualifiedNameSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitQueryBody(QueryBodySyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitQueryContinuation(QueryContinuationSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitQueryExpression(QueryExpressionSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitRefExpression(RefExpressionSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitRefType(RefTypeSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitRefTypeExpression(RefTypeExpressionSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitRefValueExpression(RefValueExpressionSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitReferenceDirectiveTrivia(ReferenceDirectiveTriviaSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitRegionDirectiveTrivia(RegionDirectiveTriviaSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitSelectClause(SelectClauseSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitShebangDirectiveTrivia(ShebangDirectiveTriviaSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitSimpleBaseType(SimpleBaseTypeSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitSimpleLambdaExpression(SimpleLambdaExpressionSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitSingleVariableDesignation(SingleVariableDesignationSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitSizeOfExpression(SizeOfExpressionSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitSkippedTokensTrivia(SkippedTokensTriviaSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitStackAllocArrayCreationExpression(StackAllocArrayCreationExpressionSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitStructDeclaration(StructDeclarationSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitSwitchSection(SwitchSectionSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitSwitchStatement(SwitchStatementSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitThisExpression(ThisExpressionSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitThrowExpression(ThrowExpressionSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitThrowStatement(ThrowStatementSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitTryStatement(TryStatementSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitTupleElement(TupleElementSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitTupleExpression(TupleExpressionSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitTupleType(TupleTypeSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitTypeArgumentList(TypeArgumentListSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitTypeConstraint(TypeConstraintSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitTypeCref(TypeCrefSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitTypeOfExpression(TypeOfExpressionSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitTypeParameter(TypeParameterSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitTypeParameterConstraintClause(TypeParameterConstraintClauseSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitTypeParameterList(TypeParameterListSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitUndefDirectiveTrivia(UndefDirectiveTriviaSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitUnsafeStatement(UnsafeStatementSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitUsingDirective(UsingDirectiveSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitUsingStatement(UsingStatementSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitVariableDeclaration(VariableDeclarationSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitVariableDeclarator(VariableDeclaratorSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitWarningDirectiveTrivia(WarningDirectiveTriviaSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitWhenClause(WhenClauseSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitWhereClause(WhereClauseSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitWhileStatement(WhileStatementSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitXmlCDataSection(XmlCDataSectionSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitXmlComment(XmlCommentSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitXmlCrefAttribute(XmlCrefAttributeSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitXmlElement(XmlElementSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitXmlElementEndTag(XmlElementEndTagSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitXmlElementStartTag(XmlElementStartTagSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitXmlEmptyElement(XmlEmptyElementSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitXmlName(XmlNameSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitXmlNameAttribute(XmlNameAttributeSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitXmlPrefix(XmlPrefixSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitXmlProcessingInstruction(XmlProcessingInstructionSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitXmlText(XmlTextSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitXmlTextAttribute(XmlTextAttributeSyntax node)
        {
            throw new NotImplementedException();
        }

        public override void VisitYieldStatement(YieldStatementSyntax node)
        {
            throw new NotImplementedException();
        }

#endregion
    }
}
