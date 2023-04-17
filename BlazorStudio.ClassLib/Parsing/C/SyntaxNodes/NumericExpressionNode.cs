using System.Collections.Immutable;

namespace BlazorStudio.ClassLib.Parsing.C.SyntaxNodes;

public abstract class NumericExpressionNode : ISyntaxNode
{
    public abstract SyntaxKind SyntaxKind { get; }
    public abstract ImmutableArray<ISyntax> Children { get; }
}