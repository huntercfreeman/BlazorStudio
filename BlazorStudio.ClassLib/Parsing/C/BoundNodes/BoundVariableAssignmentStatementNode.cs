using BlazorStudio.ClassLib.Parsing.C.SyntaxNodes;
using BlazorStudio.ClassLib.Parsing.C.SyntaxNodes.Expression;
using BlazorStudio.ClassLib.Parsing.C.SyntaxTokens;
using System.Collections.Immutable;

namespace BlazorStudio.ClassLib.Parsing.C.BoundNodes;

public class BoundVariableAssignmentStatementNode : ISyntaxNode
{
    public BoundVariableAssignmentStatementNode(
        ISyntaxToken identifierToken,
        IExpressionNode rightHandExpression)
    {
        IdentifierToken = identifierToken;
        RightHandExpression = rightHandExpression;

        Children = new ISyntax[]
        {
            IdentifierToken,
            RightHandExpression
        }.ToImmutableArray();
    }

    public ImmutableArray<ISyntax> Children { get; }
    public SyntaxKind SyntaxKind => SyntaxKind.BoundVariableAssignmentStatementNode;

    public ISyntaxToken IdentifierToken { get; }
    public IExpressionNode RightHandExpression { get; }
}
