using System.Collections.Immutable;

namespace BlazorStudio.ClassLib.Parsing.C.SyntaxNodes;

[Obsolete("See TypedBinaryExpressionNode")]
public class NumericThreePartExpressionNode : NumericExpressionNode
{
    public NumericThreePartExpressionNode(
        NumericExpressionNode? leftNumericExpressionNode,
        OperatorNode operatorNode,
        NumericExpressionNode? rightNumericExpressionNode)
    {
        LeftNumericExpressionNode = leftNumericExpressionNode;
        OperatorNode = operatorNode;
        RightNumericExpressionNode = rightNumericExpressionNode;
    }

    public NumericExpressionNode? LeftNumericExpressionNode { get; }
    public OperatorNode OperatorNode { get; }
    public NumericExpressionNode? RightNumericExpressionNode { get; }
    public override SyntaxKind SyntaxKind => SyntaxKind.NumericThreePartExpressionNode;
    public override ImmutableArray<ISyntax> Children => new ISyntax[]
    {
        LeftNumericExpressionNode,
        OperatorNode,
        RightNumericExpressionNode,
    }.ToImmutableArray();
}