using System.Collections.Immutable;

namespace BlazorStudio.ClassLib.Parsing.C.SyntaxNodes;

public class TypedUnaryExpressionNode : TypedExpressionNode
{
    public TypedUnaryExpressionNode(
        TypedExpressionNode operand,
        OperatorNode @operator,
        Type resultType)
    {
        Operand = operand;
        Operator = @operator;
        ResultType = resultType;
    }

    public TypedExpressionNode Operand { get; }
    public OperatorNode Operator { get; }
    
    public override Type ResultType { get; }
    public override SyntaxKind SyntaxKind => SyntaxKind.TypedUnaryExpressionNode;
    public override ImmutableArray<ISyntax> Children => new ISyntax[]
    {
        Operand,
        Operator
    }.ToImmutableArray();
}