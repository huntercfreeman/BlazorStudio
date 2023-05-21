using BlazorStudio.ClassLib.Parsing.C.BoundNodes.Expression;
using BlazorStudio.ClassLib.Parsing.C.SyntaxNodes;
using BlazorStudio.ClassLib.Parsing.C.SyntaxTokens;
using System.Collections.Immutable;

namespace BlazorStudio.ClassLib.Parsing.C.BoundNodes.Statements;

public class BoundVariableAssignmentStatementNode : ISyntaxNode
{
    public BoundVariableAssignmentStatementNode(
        ISyntaxToken identifierToken,
        IBoundExpressionNode boundExpressionNode)
    {
        IdentifierToken = identifierToken;
        BoundExpressionNode = boundExpressionNode;

        Children = new ISyntax[]
        {
            IdentifierToken,
            BoundExpressionNode
        }.ToImmutableArray();
    }

    public ISyntaxToken IdentifierToken { get; }
    public IBoundExpressionNode BoundExpressionNode { get; }

    public ImmutableArray<ISyntax> Children { get; }
    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.BoundVariableAssignmentStatementNode;
}
