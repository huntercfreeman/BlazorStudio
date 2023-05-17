using System.Collections.Immutable;

namespace BlazorStudio.ClassLib.Parsing.C.SyntaxNodes;

public class CompilationUnitBuilder
{
    public CompilationUnitBuilder(CompilationUnitBuilder? parent)
    {
        Parent = parent;
    }

    public bool IsExpression { get; set; }
    public List<ISyntax> Children { get; set; } = new();
    public CompilationUnitBuilder? Parent { get; }

    public CompilationUnit Build()
    {
        return new CompilationUnit(
            IsExpression,
            Children.ToImmutableArray());
    }
}