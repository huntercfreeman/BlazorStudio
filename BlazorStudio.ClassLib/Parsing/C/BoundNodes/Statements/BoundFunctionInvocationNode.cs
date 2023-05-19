using BlazorStudio.ClassLib.Parsing.C.SyntaxNodes;
using BlazorStudio.ClassLib.Parsing.C.SyntaxTokens;
using System.Collections.Immutable;

namespace BlazorStudio.ClassLib.Parsing.C.BoundNodes.Statements;

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

    public ISyntaxToken IdentifierToken { get; }

    public ImmutableArray<ISyntax> Children { get; }
    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.BoundFunctionInvocationNode;
}