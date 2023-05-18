using BlazorStudio.ClassLib.Parsing.C.SyntaxNodes;
using BlazorStudio.ClassLib.Parsing.C.SyntaxTokens;
using System.Collections.Immutable;

namespace BlazorStudio.ClassLib.Parsing.C.BoundNodes;

public class BoundFunctionInvocationNode : ISyntaxNode
{
    public BoundFunctionInvocationNode(
        ISyntaxToken identifierToken)
    {
        IdentifierToken = identifierToken;

        Children = new ISyntax[]
        {
            IdentifierToken
        }.ToImmutableArray();
    }

    public ImmutableArray<ISyntax> Children { get; }
    public SyntaxKind SyntaxKind => SyntaxKind.BoundFunctionInvocationNode;

    public ISyntaxToken IdentifierToken { get; }
}