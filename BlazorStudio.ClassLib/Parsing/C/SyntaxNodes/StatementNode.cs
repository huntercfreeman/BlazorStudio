using System.Collections.Immutable;
using BlazorStudio.ClassLib.Parsing.C.Symbols;
using BlazorStudio.ClassLib.Parsing.C.SyntaxTokens;

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