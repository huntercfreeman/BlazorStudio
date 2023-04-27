using System.Collections.Immutable;

namespace BlazorStudio.ClassLib.Parsing.C.SyntaxNodes;

public abstract class TypedExpressionNode : ISyntaxNode
{
    public abstract Type ResultType { get; } 
    public abstract SyntaxKind SyntaxKind { get; } 
    public abstract ImmutableArray<ISyntax> Children { get; }
}