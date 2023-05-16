using BlazorStudio.ClassLib.Parsing.C.SyntaxNodes;
using BlazorStudio.ClassLib.Parsing.C.SyntaxTokens;
using System.Collections.Immutable;

namespace BlazorStudio.ClassLib.Parsing.C.BoundNodes;

public class BoundFunctionDeclarationNode : ISyntaxNode
{
    public BoundFunctionDeclarationNode(
        BoundTypeNode boundTypeNode,
        ISyntaxToken identifierToken)
    {
        BoundTypeNode = boundTypeNode;
        IdentifierToken = identifierToken;
    }

    public ImmutableArray<ISyntax> Children { get; }
    public SyntaxKind SyntaxKind => SyntaxKind.BoundFunctionDeclarationNode;

    public BoundTypeNode BoundTypeNode { get; }
    public ISyntaxToken IdentifierToken { get; }
}
