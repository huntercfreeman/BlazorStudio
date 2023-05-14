using System.Collections.Immutable;

namespace BlazorStudio.ClassLib.Parsing.C.SyntaxNodes;

public class BoundCompilationUnitBuilder
{
    public bool IsExpression { get; set; }
    public List<ISyntax> Children { get; set; } = new();

    public BoundCompilationUnit Build()
    {
        return new BoundCompilationUnit(
            IsExpression,
            Children.ToImmutableArray());
    }
}