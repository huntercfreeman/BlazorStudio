using System.Collections.Immutable;

namespace BlazorStudio.ClassLib.Parsing.C.SyntaxNodes;

public class TypedBinaryExpressionNode : TypedExpressionNode
{
    public TypedBinaryExpressionNode(
        TypedExpressionNode leftOperand,
        OperatorNode @operator,
        TypedExpressionNode rightOperand,
        Type resultType)
    {
        LeftOperand = leftOperand;
        Operator = @operator;
        RightOperand = rightOperand;
        ResultType = resultType;
    }

    public TypedExpressionNode LeftOperand { get; }
    public OperatorNode Operator { get; }
    public TypedExpressionNode RightOperand { get; }
    
    public override Type ResultType { get; } 
    public override SyntaxKind SyntaxKind => SyntaxKind.TypedBinaryExpressionNode; 
    public override ImmutableArray<ISyntax> Children => new ISyntax[]
    {
        LeftOperand,
        Operator,
        RightOperand,
    }.ToImmutableArray();
}