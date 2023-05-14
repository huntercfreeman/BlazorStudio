using System.Collections.Immutable;
using BlazorCommon.RazorLib.Icons.Codicon;
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
                ParseNumericLiteralToken((NumericLiteralToken)currentToken);
                break;
        }

        return _compilationUnitBuilder.Build(_binder.BoundCompilationUnit);
    }

    private void ParseNumericLiteralToken(NumericLiteralToken token)
    {
        var literalExpressionNode = new LiteralExpressionNode(token);

        if (_currentNode is null)
        {
            _compilationUnitBuilder.IsExpression = true;
            _compilationUnitBuilder.Children.Add(literalExpressionNode);

            _currentNode = literalExpressionNode;

            _binder.BindLiteralExpressionNode((LiteralExpressionNode)_currentNode);
        }
        else
        {
            throw new ApplicationException("TODO");
        }
    }
}
