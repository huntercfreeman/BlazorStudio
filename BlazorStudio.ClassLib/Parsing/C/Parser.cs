using System.Collections.Immutable;
using BlazorStudio.ClassLib.Parsing.C.BoundNodes.Expression;
using BlazorStudio.ClassLib.Parsing.C.SyntaxNodes;
using BlazorStudio.ClassLib.Parsing.C.SyntaxNodes.Expression;
using BlazorStudio.ClassLib.Parsing.C.SyntaxNodes.Statement;
using BlazorStudio.ClassLib.Parsing.C.SyntaxTokens;

namespace BlazorStudio.ClassLib.Parsing.C;

public class Parser
{
    private readonly TokenWalker _tokenWalker;
    private readonly Binder _binder;

    public Parser(
        ImmutableArray<ISyntaxToken> tokens,
        string sourceText)
    {
        _tokenWalker = new TokenWalker(tokens);
        _binder = new Binder(sourceText);
    }

    private ISyntaxNode? _nodeCurrent;
    private CompilationUnitBuilder _compilationUnitBuilder = new();

    public CompilationUnit Parse()
    {
        while (true)
        {
            var tokenCurrent = _tokenWalker.Consume();

            switch (tokenCurrent.SyntaxKind)
            {
                case SyntaxKind.NumericLiteralToken:
                    _ = ParseNumericLiteralToken((NumericLiteralToken)tokenCurrent);
                    break;
                case SyntaxKind.StringLiteralToken:
                    _ = ParseStringLiteralToken((StringLiteralToken)tokenCurrent);
                    break;
                case SyntaxKind.PlusToken:
                    ParsePlusToken((PlusToken)tokenCurrent);
                    break;
                case SyntaxKind.PreprocessorDirectiveToken:
                    ParsePreprocessorDirectiveToken((PreprocessorDirectiveToken)tokenCurrent);
                    break;
                case SyntaxKind.CommentSingleLineToken:
                    // Do not parse comments.
                    break;
                case SyntaxKind.KeywordToken:
                    ParseKeywordToken((KeywordToken)tokenCurrent);
                    break;
                case SyntaxKind.IdentifierToken:
                    ParseIdentifierToken((IdentifierToken)tokenCurrent);
                    break;
                case SyntaxKind.EndOfFileToken:
                    if (_nodeCurrent is IExpressionNode)
                    {
                        _compilationUnitBuilder.IsExpression = true;
                        _compilationUnitBuilder.Children.Add(_nodeCurrent);
                    }
                    break;
            }

            if (tokenCurrent.SyntaxKind == SyntaxKind.EndOfFileToken)
                break;
        }

        return _compilationUnitBuilder.Build();
    }

    private BoundLiteralExpressionNode ParseNumericLiteralToken(NumericLiteralToken token)
    {
        var literalExpressionNode = new LiteralExpressionNode(token);

        var boundLiteralExpressionNode = _binder
            .BindLiteralExpressionNode(literalExpressionNode);

        _nodeCurrent = boundLiteralExpressionNode;

        return boundLiteralExpressionNode;
    }
    
    private BoundLiteralExpressionNode ParseStringLiteralToken(StringLiteralToken token)
    {
        var literalExpressionNode = new LiteralExpressionNode(token);

        var boundLiteralExpressionNode = _binder
                .BindLiteralExpressionNode(literalExpressionNode);

        _nodeCurrent = boundLiteralExpressionNode;

        return boundLiteralExpressionNode;
    }
    
    private BoundBinaryExpressionNode ParsePlusToken(PlusToken token)
    {
        var localNodeCurrent = _nodeCurrent;

        if (localNodeCurrent is not BoundLiteralExpressionNode leftBoundLiteralExpressionNode)
            throw new NotImplementedException();

        var nextToken = _tokenWalker.Consume();

        BoundLiteralExpressionNode rightBoundLiteralExpressionNode;

        if (nextToken.SyntaxKind == SyntaxKind.NumericLiteralToken)
        {
            rightBoundLiteralExpressionNode = ParseNumericLiteralToken(
                (NumericLiteralToken)nextToken);
        }
        else
        {
            rightBoundLiteralExpressionNode = ParseStringLiteralToken(
                (StringLiteralToken)nextToken);
        }

        var boundBinaryOperatorNode = _binder.BindBinaryOperatorNode(
            leftBoundLiteralExpressionNode,
            token,
            rightBoundLiteralExpressionNode);

        var boundBinaryExpressionNode = new BoundBinaryExpressionNode(
            leftBoundLiteralExpressionNode,
            boundBinaryOperatorNode,
            rightBoundLiteralExpressionNode);

        _nodeCurrent = boundBinaryExpressionNode;

        return boundBinaryExpressionNode;
    }

    private IStatementNode ParsePreprocessorDirectiveToken(PreprocessorDirectiveToken token)
    {
        var nextToken = _tokenWalker.Consume();

        if (nextToken.SyntaxKind == SyntaxKind.LibraryReferenceToken)
        {
            var preprocessorLibraryReferenceStatement = new PreprocessorLibraryReferenceStatement(
                token,
                nextToken);

            _compilationUnitBuilder.Children.Add(preprocessorLibraryReferenceStatement);

            return preprocessorLibraryReferenceStatement;
        }
        else
        {
            throw new NotImplementedException();
        }
    }
    
    private void ParseKeywordToken(KeywordToken token)
    {
        if (_binder.TryBindTypeNode(token, out var boundTypeNode) &&
            boundTypeNode is not null)
        {
            // 'int', 'string', 'bool', etc...
            _nodeCurrent = boundTypeNode;
        }
        else
        {
            throw new NotImplementedException();
        }
    }
    
    private void ParseIdentifierToken(IdentifierToken token)
    {
        if (_nodeCurrent is not null &&
            _nodeCurrent.SyntaxKind == SyntaxKind.BoundTypeNode)
        {
            // 'int main()...', 'char c', 'int num', etc...
            
        }
        else
        {
            throw new NotImplementedException();
        }
    }
}
