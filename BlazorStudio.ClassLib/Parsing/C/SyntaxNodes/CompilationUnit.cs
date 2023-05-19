using System.Collections.Immutable;

namespace BlazorStudio.ClassLib.Parsing.C.SyntaxNodes;

public class CompilationUnit : ISyntaxNode
{
    public CompilationUnit(
        bool isExpression,
        ImmutableArray<ISyntax> children)
    {
        IsExpression = isExpression;
        Children = children;
        Diagnostics = ImmutableArray<BlazorStudioDiagnostic>.Empty;
    }

    public CompilationUnit(
        bool isExpression,
        ImmutableArray<ISyntax> children,
        ImmutableArray<BlazorStudioDiagnostic> diagnostics)
    {
        IsExpression = isExpression;
        Children = children;
        Diagnostics = diagnostics;
    }

    public bool IsExpression { get; }
    public ImmutableArray<BlazorStudioDiagnostic> Diagnostics { get; }

    public ImmutableArray<ISyntax> Children { get; }
    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.CompilationUnit;
}
