using BlazorStudio.ClassLib.Parsing.C.SyntaxNodes.Expression;
using BlazorStudio.ClassLib.Parsing.C.BoundNodes.Expression;
using BlazorStudio.ClassLib.Parsing.C.SyntaxTokens;
using BlazorStudio.ClassLib.Parsing.C.BoundNodes;
using BlazorStudio.ClassLib.Parsing.C.Scope;

namespace BlazorStudio.ClassLib.Parsing.C;

public class Binder
{
    private readonly BoundScope _globalScope = CLanguageFacts.Scope.GetInitialGlobalScope();
    private readonly string _sourceText;

    public Binder(string sourceText)
    {
        _sourceText = sourceText;
    }

    public BoundLiteralExpressionNode BindLiteralExpressionNode(LiteralExpressionNode literalExpressionNode)
    {
        var type = literalExpressionNode.LiteralSyntaxToken.SyntaxKind switch
        {
            SyntaxKind.NumericLiteralToken => typeof(int),
            SyntaxKind.StringLiteralToken => typeof(string),
            _ => throw new NotImplementedException(),
        };

        var boundLiteralExpressionNode = new BoundLiteralExpressionNode(
            literalExpressionNode.LiteralSyntaxToken,
            type);

        return boundLiteralExpressionNode;
    }

    public BoundBinaryOperatorNode BindBinaryOperatorNode(
        BoundLiteralExpressionNode leftBoundLiteralExpressionNode,
        ISyntaxToken operatorToken,
        BoundLiteralExpressionNode rightBoundLiteralExpressionNode)
    {
        if (leftBoundLiteralExpressionNode.ResultType == typeof(int) &&
            rightBoundLiteralExpressionNode.ResultType == typeof(int))
        {
            switch (operatorToken.SyntaxKind)
            {
                case SyntaxKind.PlusToken:
                    return new BoundBinaryOperatorNode(
                        leftBoundLiteralExpressionNode.ResultType,
                        operatorToken,
                        rightBoundLiteralExpressionNode.ResultType,
                        typeof(int));
            }
        }
        else if (leftBoundLiteralExpressionNode.ResultType == typeof(string) &&
            rightBoundLiteralExpressionNode.ResultType == typeof(string))
        {
            switch (operatorToken.SyntaxKind)
            {
                case SyntaxKind.PlusToken:
                    return new BoundBinaryOperatorNode(
                        leftBoundLiteralExpressionNode.ResultType,
                        operatorToken,
                        rightBoundLiteralExpressionNode.ResultType,
                        typeof(string));
            }
        }

        throw new NotImplementedException();
    }
    
    public bool TryBindTypeNode(
        ISyntaxToken token,
        out BoundTypeNode? boundTypeNode)
    {
        var text = token.BlazorStudioTextSpan.GetText(_sourceText);

        if (_globalScope.TypeMap.TryGetValue(text, out var type))
        {
            boundTypeNode = new BoundTypeNode(type, token);
            return true;
        }

        return false;
    }
}
