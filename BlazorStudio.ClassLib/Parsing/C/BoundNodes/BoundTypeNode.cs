using BlazorStudio.ClassLib.Parsing.C.SyntaxNodes;
using BlazorStudio.ClassLib.Parsing.C.SyntaxTokens;
using System.Collections.Immutable;

namespace BlazorStudio.ClassLib.Parsing.C.BoundNodes;

public class BoundTypeNode : ISyntaxNode
{
    public BoundTypeNode(
        Type type,
        ISyntaxToken token)
    {
        Type = type;
        Token = token;
    }

    public ImmutableArray<ISyntax> Children { get; }
    public SyntaxKind SyntaxKind => SyntaxKind.BoundTypeNode;

    public Type Type { get; }
    public ISyntaxToken Token { get; }
}
