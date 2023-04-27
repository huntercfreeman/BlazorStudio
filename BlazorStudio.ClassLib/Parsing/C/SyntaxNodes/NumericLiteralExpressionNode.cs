using System.Collections.Immutable;
using BlazorStudio.ClassLib.Parsing.C.SyntaxTokens;

namespace BlazorStudio.ClassLib.Parsing.C.SyntaxNodes;

[Obsolete("See TypedLiteralExpressionNode")]
public class NumericLiteralExpressionNode : NumericExpressionNode
{
    public NumericLiteralExpressionNode(
        NumericLiteralToken numericLiteralToken)
    {
        NumericLiteralToken = numericLiteralToken;
    }

    public NumericLiteralToken NumericLiteralToken { get; }
    public override SyntaxKind SyntaxKind => SyntaxKind.NumericLiteralExpressionNode;
    public override ImmutableArray<ISyntax> Children => new ISyntax[]
    {
        NumericLiteralToken
    }.ToImmutableArray();
}