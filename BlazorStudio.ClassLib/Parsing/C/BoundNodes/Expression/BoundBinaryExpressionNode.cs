using BlazorStudio.ClassLib.Parsing.C.SyntaxNodes.Expression;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorStudio.ClassLib.Parsing.C.BoundNodes.Expression;

public class BoundBinaryExpressionNode : IBoundExpressionNode
{
    public BoundBinaryExpressionNode(
        IBoundExpressionNode leftBoundExpressionNode,
        BoundBinaryOperatorNode boundBinaryOperatorNode,
        IBoundExpressionNode rightBoundExpressionNode)
    {
        LeftBoundExpressionNode = leftBoundExpressionNode;
        BoundBinaryOperatorNode = boundBinaryOperatorNode;
        RightBoundExpressionNode = rightBoundExpressionNode;

        Children = new ISyntax[]
        {
            LeftBoundExpressionNode,
            BoundBinaryOperatorNode,
            RightBoundExpressionNode
        }.ToImmutableArray();
    }

    public ImmutableArray<ISyntax> Children { get; }
    public SyntaxKind SyntaxKind => SyntaxKind.BoundBinaryExpressionNode;

    public Type ResultType => BoundBinaryOperatorNode.ResultType;

    public IBoundExpressionNode LeftBoundExpressionNode { get; }
    public BoundBinaryOperatorNode BoundBinaryOperatorNode { get; }
    public IBoundExpressionNode RightBoundExpressionNode { get; }
}
