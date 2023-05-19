using BlazorStudio.ClassLib.Parsing.C.BoundNodes.Expression;
using BlazorStudio.ClassLib.Parsing.C.SyntaxNodes;
using BlazorStudio.ClassLib.Parsing.C.SyntaxNodes.Expression;
using BlazorStudio.ClassLib.Parsing.C.SyntaxTokens;
using System.Collections.Immutable;

namespace BlazorStudio.ClassLib.Parsing.C.BoundNodes.Statements;

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
