using System.Collections.Immutable;

namespace BlazorStudio.ClassLib.Parsing.C.SyntaxNodes;

public class PreprocessorDirectiveNode : ISyntaxNode
{
    public PreprocessorDirectiveNode(
        ImmutableArray<ISyntax> children)
    {
        Children = children;
    }
    
    public ImmutableArray<ISyntax> Children { get; }
    public SyntaxKind SyntaxKind => SyntaxKind.PreprocessorDirectiveNode;
}