using System.Collections.Immutable;
using BlazorCommon.RazorLib.Icons.Codicon;
using BlazorStudio.ClassLib.Parsing.C.BoundNodes;
using BlazorStudio.ClassLib.Parsing.C.BoundNodes.Expression;
using BlazorStudio.ClassLib.Parsing.C.SyntaxNodes;
using BlazorStudio.ClassLib.Parsing.C.SyntaxNodes.Expression;
using BlazorStudio.ClassLib.Parsing.C.SyntaxTokens;

namespace BlazorStudio.ClassLib.Parsing.C;

public class Parser
{
    private readonly TokenWalker _tokenWalker;
    private readonly Binder _binder;
    
    public Parser(ImmutableArray<ISyntaxToken> tokens)
    {
        _tokenWalker = new TokenWalker(tokens);
        _binder = new Binder();
    }

    private ISyntaxNode? _currentNode;
    private CompilationUnitBuilder _compilationUnitBuilder = new();

    private ISyntaxToken Current => _tokenWalker.Peek(0);
    private ISyntaxToken Next => _tokenWalker.Peek(1);

    public CompilationUnit Parse()
    {
        var currentToken = Current;

        switch (currentToken.SyntaxKind)
        {
            case SyntaxKind.NumericLiteralToken:
                _ = ParseNumericLiteralToken((NumericLiteralToken)currentToken);
                break;
            case SyntaxKind.StringLiteralToken:
                _ = ParseStringLiteralToken((StringLiteralToken)currentToken);
                break;
            case SyntaxKind.PlusToken:
                ParsePlusToken((PlusToken)currentToken);
                break;
        }
        return _compilationUnitBuilder.Build();
    }

    private BoundLiteralExpressionNode ParseNumericLiteralToken(NumericLiteralToken token)
    {
        var literalExpressionNode = new LiteralExpressionNode(token);

        if (_currentNode is null)
        {
            var boundLiteralExpressionNode = _binder
                .BindLiteralExpressionNode(literalExpressionNode);

            _currentNode = boundLiteralExpressionNode;

            _compilationUnitBuilder.IsExpression = true;

            if (_tokenWalker.Tokens.Length == 1)
                _compilationUnitBuilder.Children.Add(boundLiteralExpressionNode);

            return boundLiteralExpressionNode;
        }
        else
        {
            throw new ApplicationException("TODO");
        }
    }
    
    private BoundLiteralExpressionNode ParseStringLiteralToken(StringLiteralToken token)
    {
        var literalExpressionNode = new LiteralExpressionNode(token);

        if (_currentNode is null)
        {
            var boundLiteralExpressionNode = _binder
                .BindLiteralExpressionNode(literalExpressionNode);

            _currentNode = boundLiteralExpressionNode;

            _compilationUnitBuilder.IsExpression = true;
            _compilationUnitBuilder.Children.Add(literalExpressionNode);

            return boundLiteralExpressionNode;
        }
        else
        {
            throw new ApplicationException("TODO");
        }
    }
    
    private BoundBinaryExpressionNode ParsePlusToken(PlusToken token)
    {
        var currentToken = Current;

        if (currentToken is not BoundLiteralExpressionNode leftBoundLiteralExpressionNode)
            throw new NotImplementedException();

        var nextToken = Next;

        var rightBoundLiteralExpressionNode = ParseNumericLiteralToken(
            (NumericLiteralToken)nextToken);

        var boundBinaryOperatorNode = _binder.BindBinaryOperatorNode(
            leftBoundLiteralExpressionNode,
            token,
            rightBoundLiteralExpressionNode);

        var boundBinaryExpressionNode = new BoundBinaryExpressionNode(
            leftBoundLiteralExpressionNode,
            boundBinaryOperatorNode,
            rightBoundLiteralExpressionNode);

        _compilationUnitBuilder.Children.Add(boundBinaryExpressionNode);

        return boundBinaryExpressionNode;
    }
}
