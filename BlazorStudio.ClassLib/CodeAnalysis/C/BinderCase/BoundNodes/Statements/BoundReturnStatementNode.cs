using BlazorStudio.ClassLib.CodeAnalysis.C.BinderCase.BoundNodes.Expression;
using BlazorStudio.ClassLib.CodeAnalysis.C.Syntax;
using BlazorStudio.ClassLib.CodeAnalysis.C.Syntax.SyntaxNodes;
using BlazorStudio.ClassLib.CodeAnalysis.C.Syntax.SyntaxTokens;
using System.Collections.Immutable;

namespace BlazorStudio.ClassLib.CodeAnalysis.C.BinderCase.BoundNodes.Statements;

public class BoundReturnStatementNode : ISyntaxNode
{
    public BoundReturnStatementNode(
        KeywordToken keywordToken,
        IBoundExpressionNode boundExpressionNode)
    {
        KeywordToken = keywordToken;
        BoundExpressionNode = boundExpressionNode;

        Children = new ISyntax[]
        {
            KeywordToken,
            BoundExpressionNode
        }.ToImmutableArray();
    }

    public KeywordToken KeywordToken { get; }
    public IBoundExpressionNode BoundExpressionNode { get; }

    public ImmutableArray<ISyntax> Children { get; }
    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.BoundReturnStatementNode;
}
