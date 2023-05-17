using System.Collections.Immutable;

namespace BlazorStudio.ClassLib.Parsing.C.SyntaxNodes;

public class CompilationUnit : ISyntax
{
    public CompilationUnit(
        bool isExpression,
        ImmutableArray<ISyntax> children)
    {
        IsExpression = isExpression;
        Children = children;
    }

    public bool IsExpression { get; }
    public ImmutableArray<ISyntax> Children { get; }

    public SyntaxKind SyntaxKind => SyntaxKind.CompilationUnit;
}
