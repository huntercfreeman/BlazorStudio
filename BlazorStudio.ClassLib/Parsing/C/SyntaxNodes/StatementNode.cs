using System.Collections.Immutable;

namespace BlazorStudio.ClassLib.Parsing.C.SyntaxNodes;

public class StatementNode : ISyntaxNode
{
    public StatementNode(
        ISyntaxNode node)
    {
        Node = node;
    }
    
    public ISyntaxNode Node { get; }

    public ImmutableArray<ISyntax> Children => new ISyntax[]
    {
        Node,
    }.ToImmutableArray();
    public SyntaxKind SyntaxKind => SyntaxKind.StatementNode;
}