using System.Collections.Immutable;
using BlazorStudio.ClassLib.Parsing.C.BoundNodes;
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

    private ISyntaxNode? _nodeRecent;
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
                case SyntaxKind.OpenBraceToken:
                    ParseOpenBraceToken((OpenBraceToken)tokenCurrent);
                    break;
                case SyntaxKind.EndOfFileToken:
                    if (_nodeRecent is IExpressionNode)
                    {
                        _compilationUnitBuilder.IsExpression = true;
                        _compilationUnitBuilder.Children.Add(_nodeRecent);
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

        _nodeRecent = boundLiteralExpressionNode;

        return boundLiteralExpressionNode;
    }
    
    private BoundLiteralExpressionNode ParseStringLiteralToken(StringLiteralToken token)
    {
        var literalExpressionNode = new LiteralExpressionNode(token);

        var boundLiteralExpressionNode = _binder
                .BindLiteralExpressionNode(literalExpressionNode);

        _nodeRecent = boundLiteralExpressionNode;

        return boundLiteralExpressionNode;
    }
    
    private BoundBinaryExpressionNode ParsePlusToken(PlusToken token)
    {
        var localNodeCurrent = _nodeRecent;

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

        _nodeRecent = boundBinaryExpressionNode;

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
            _nodeRecent = boundTypeNode;
        }
        else
        {
            throw new NotImplementedException();
        }
    }
    
    private void ParseIdentifierToken(IdentifierToken token)
    {
        if (_nodeRecent is not null &&
            _nodeRecent.SyntaxKind == SyntaxKind.BoundTypeNode)
        {
            // 'int main()...', 'char c', 'int num', etc...
            var nextToken = _tokenWalker.Consume();

            if (nextToken.SyntaxKind == SyntaxKind.OpenParenthesisToken)
            {
                var boundFunctionDeclarationNode = _binder.BindFunctionDeclarationNode(
                    (BoundTypeNode)_nodeRecent,
                    token);

                _nodeRecent = boundFunctionDeclarationNode;

                ParseFunctionArguments();
            }
        }
        else
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// TODO: Implement ParseFunctionArguments() correctly. Until then, skip until the body of the function is found. Specifically until the CloseParenthesisToken is found
    /// </summary>
    private void ParseFunctionArguments()
    {
        while (true)
        {
            var tokenCurrent = _tokenWalker.Consume();

            if (tokenCurrent.SyntaxKind == SyntaxKind.EndOfFileToken ||
                tokenCurrent.SyntaxKind == SyntaxKind.CloseParenthesisToken)
            { 
                break;
            }
        }
    }
    
    private void ParseOpenBraceToken(OpenBraceToken token)
    {
        if (_nodeRecent is not null &&
            _nodeRecent.SyntaxKind == SyntaxKind.BoundFunctionDeclarationNode)
        {
            // Function body is being defined

        }
    }
}
