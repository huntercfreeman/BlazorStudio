using System.Collections.Immutable;
using BlazorStudio.ClassLib.Parsing.C.SyntaxTokens;

namespace BlazorStudio.ClassLib.Parsing.C.SyntaxNodes;

public class OperatorAssignmentNode : OperatorNode, ISyntaxNode
{
    public OperatorAssignmentNode(
        EqualsToken equalsToken)
    {
        EqualsToken = equalsToken;
    }

    public EqualsToken EqualsToken { get; }
    public override SyntaxKind SyntaxKind => SyntaxKind.OperatorAdditionNode;
    public override ImmutableArray<ISyntax> Children => new ISyntax[]
    {
        EqualsToken
    }.ToImmutableArray();
}