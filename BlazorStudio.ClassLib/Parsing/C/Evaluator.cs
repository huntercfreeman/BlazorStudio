using BlazorStudio.ClassLib.Parsing.C.SyntaxNodes;

namespace BlazorStudio.ClassLib.Parsing.C;

public class Evaluator
{
    private readonly ISyntaxNode _root;
    private readonly string _content;

    public Evaluator(
        ISyntaxNode root,
        string content)
    {
        _root = root;
        _content = content;
    }
    
    public EvaluatorResult? Evaluate()
    {
        if (_root is NumericExpressionNode)
        {
            return EvaluateNumericExpressionNode(
                (NumericExpressionNode)_root);
        }

        return null;
    }
    
    private EvaluatorResult EvaluateNumericExpressionNode(NumericExpressionNode node)
    {
        if (node is NumericLiteralExpressionNode numericLiteralExpressionNode)
        {
            var numericValueAsText = numericLiteralExpressionNode.NumericLiteralToken.BlazorStudioTextSpan
                .GetText(_content);

            var numericValue = int.Parse(numericValueAsText);

            return new EvaluatorResult(
                typeof(int),
                numericValue);
        }
        
        if (node is NumericThreePartExpressionNode numericThreePartExpressionNode)
        {
            var leftValue = EvaluateNumericExpressionNode(
                numericThreePartExpressionNode.LeftNumericExpressionNode!);
            
            var rightValue = EvaluateNumericExpressionNode(
                numericThreePartExpressionNode.RightNumericExpressionNode!);

            var operationResult = EvaluateNumericOperation(
                (int)leftValue.ResultValue,
                numericThreePartExpressionNode.OperatorNode,
                (int)rightValue.ResultValue);

            return new EvaluatorResult(
                typeof(int),
                operationResult);
        }

        throw new NotImplementedException();
    }
    
    private EvaluatorResult EvaluateNumericOperation(
        int leftValue,
        OperatorNode operatorNode,
        int rightValue)
    {
        Type resultType;
        object resultValue;

        switch (operatorNode.SyntaxKind)
        {
            case SyntaxKind.OperatorAdditionNode:
                resultType = typeof(int);
                resultValue = leftValue + rightValue;
                break;
            default:
                throw new NotImplementedException();
        }

        return new EvaluatorResult(
            resultType,
            resultValue);
    }
}