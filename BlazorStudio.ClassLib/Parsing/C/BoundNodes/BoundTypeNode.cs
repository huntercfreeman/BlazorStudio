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

        Children = new ISyntax[]
        {
            token
        }.ToImmutableArray();
    }

    public Type Type { get; }
    public ISyntaxToken Token { get; }

    public ImmutableArray<ISyntax> Children { get; }
    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.BoundTypeNode;
}
