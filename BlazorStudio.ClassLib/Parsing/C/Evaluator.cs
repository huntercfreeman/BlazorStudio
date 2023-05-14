using BlazorStudio.ClassLib.Parsing.C.BoundNodes.Expression;
using BlazorStudio.ClassLib.Parsing.C.SyntaxNodes;
using BlazorStudio.ClassLib.Parsing.C.SyntaxNodes.Expression;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BlazorStudio.ClassLib.Parsing.C;

public class Evaluator
{
    private readonly CompilationUnit _compilationUnit;
    private readonly string _sourceText;

    public Evaluator(
        CompilationUnit compilationUnit,
        string sourceText)
    {
        _compilationUnit = compilationUnit;
        _sourceText = sourceText;
    }

    public EvaluatorResult Evaluate()
    {
        if (_compilationUnit.IsExpression)
        {
            var expressionNode = _compilationUnit.Children.Single();

            return EvaluateExpression((IExpressionNode)expressionNode);
        }

        throw new NotImplementedException();
    }
    
    public EvaluatorResult EvaluateExpression(IExpressionNode expressionNode)
    {
        switch (expressionNode.SyntaxKind)
        {
            case SyntaxKind.BoundLiteralExpressionNode:
                return EvaluateBoundLiteralExpressionNode((BoundLiteralExpressionNode)expressionNode);
            case SyntaxKind.BoundBinaryExpressionNode:
                return EvaluateBoundBinaryExpressionNode((BoundBinaryExpressionNode)expressionNode);
        }

        throw new NotImplementedException();
    }

    public EvaluatorResult EvaluateBoundLiteralExpressionNode(BoundLiteralExpressionNode boundLiteralExpressionNode)
    {
        if (boundLiteralExpressionNode.ResultType == typeof(int))
        {
            var value = int.Parse(
                boundLiteralExpressionNode.LiteralSyntaxToken.BlazorStudioTextSpan
                    .GetText(_sourceText));

            return new EvaluatorResult(
                boundLiteralExpressionNode.ResultType,
                value);
        }
        else if (boundLiteralExpressionNode.ResultType == typeof(string))
        {
            var value = new string(boundLiteralExpressionNode.LiteralSyntaxToken.BlazorStudioTextSpan
                .GetText(_sourceText)
                .Skip(1)
                .SkipLast(1)
                .ToArray());

            return new EvaluatorResult(
                boundLiteralExpressionNode.ResultType,
                value);
        }

        throw new NotImplementedException();
    }

    private EvaluatorResult EvaluateBoundBinaryExpressionNode(BoundBinaryExpressionNode boundBinaryExpressionNode)
    {
        if (boundBinaryExpressionNode.ResultType == typeof(int))
        {
            var leftValue = EvaluateExpression(
                boundBinaryExpressionNode.LeftBoundExpressionNode);
            
            var rightValue = EvaluateExpression(
                boundBinaryExpressionNode.RightBoundExpressionNode);

            if (boundBinaryExpressionNode.BoundBinaryOperatorNode.OperatorToken.SyntaxKind == SyntaxKind.PlusToken)
            {
                var resultingValue = (int)leftValue.Result + (int)rightValue.Result;

                return new EvaluatorResult(
                    boundBinaryExpressionNode.ResultType,
                    resultingValue);
            }
        }
        else if (boundBinaryExpressionNode.ResultType == typeof(string))
        {
            var leftValue = EvaluateExpression(
                boundBinaryExpressionNode.LeftBoundExpressionNode);

            var rightValue = EvaluateExpression(
                boundBinaryExpressionNode.RightBoundExpressionNode);

            if (boundBinaryExpressionNode.BoundBinaryOperatorNode.OperatorToken.SyntaxKind == SyntaxKind.PlusToken)
            {
                var resultingValue = (string)leftValue.Result + (string)rightValue.Result;

                return new EvaluatorResult(
                    boundBinaryExpressionNode.ResultType,
                    resultingValue);
            }
        }

        throw new NotImplementedException();
    }
}
