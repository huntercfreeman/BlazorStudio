using System.Collections.Immutable;

namespace BlazorStudio.ClassLib.Parsing.C.SyntaxNodes;

public class VariableAssignmentExpressionNode : TypedExpressionNode
{
    public VariableAssignmentExpressionNode(
        VariableNode variableNode,
        OperatorAssignmentNode operatorAssignmentNode,
        TypedExpressionNode? rightOperand,
        Type resultType)
    {
        VariableNode = variableNode;
        OperatorAssignmentNode = operatorAssignmentNode;
        RightOperand = rightOperand;
        ResultType = resultType;
    }

    public VariableNode VariableNode { get; }
    public OperatorAssignmentNode OperatorAssignmentNode { get; }
    public TypedExpressionNode? RightOperand { get; set; }
    
    public override Type ResultType { get; } 
    public override SyntaxKind SyntaxKind => SyntaxKind.VariableAssignmentExpressionNode; 
    public override ImmutableArray<ISyntax> Children => new ISyntax[]
    {
        VariableNode,
        OperatorAssignmentNode,
        RightOperand,
    }.ToImmutableArray();
}