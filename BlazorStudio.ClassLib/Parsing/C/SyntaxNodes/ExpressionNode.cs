using System.Collections.Immutable;

namespace BlazorStudio.ClassLib.Parsing.C.SyntaxNodes;

[Obsolete("See TypedExpressionNode")]
public abstract class ExpressionNode : ISyntaxNode
{
    public abstract SyntaxKind SyntaxKind { get; }
    public abstract ImmutableArray<ISyntax> Children { get; }
}