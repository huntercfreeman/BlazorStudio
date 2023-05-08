using System.Collections.Immutable;
using BlazorStudio.ClassLib.Parsing.C.Symbols;
using BlazorStudio.ClassLib.Parsing.C.SyntaxNodes;
using BlazorStudio.ClassLib.Parsing.C.SyntaxTokens;

namespace BlazorStudio.ClassLib.Parsing.C;

public class Parser
{
    private readonly ImmutableArray<ISyntaxToken> _tokens;
    private readonly string _sourceText;
    private readonly Stack<ISyntaxNode> _nodeStack = new();
    private readonly Dictionary<string, ITypeSymbol> _typeMap = new(CLanguageFacts.Types.DefaultTypeMap);
    private readonly Dictionary<string, Variable> _variableMap = new();
    private readonly List<StatementNode> _statementNodes = new();
    
    private int _tokenIndex;
    
    public Parser(
        ImmutableArray<ISyntaxToken> tokens,
        string sourceText)
    {
        _tokens = tokens;
        _sourceText = sourceText;
    }
    
    public Dictionary<string, Variable> VariableMap => _variableMap;
    
    public CompilationUnit Parse()
    {
        for (_tokenIndex = 0; _tokenIndex < _tokens.Length; _tokenIndex++)
        {
            var token = _tokens[_tokenIndex];
            
            switch (token.SyntaxKind)
            {
                case SyntaxKind.NumericLiteralToken:
                    ParseNumericLiteralToken((NumericLiteralToken)token);
                    break;
                // case SyntaxKind.LibraryReferenceToken:
                //     ParseLibraryReferenceToken((LibraryReferenceToken)token);
                //     break;
                case SyntaxKind.PlusToken:
                    ParsePlusToken((PlusToken)token);
                    break;
                case SyntaxKind.KeywordToken:
                    ParseKeywordToken((KeywordToken)token);
                    break;
                case SyntaxKind.IdentifierToken:
                    ParseIdentifierToken((IdentifierToken)token);
                    break;
                case SyntaxKind.EqualsToken:
                    ParseEqualsToken((EqualsToken)token);
                    break;
                case SyntaxKind.StatementDelimiterToken:
                    ParseStatementDelimiterToken((StatementDelimiterToken)token);
                    break;
            }
        }

        return new CompilationUnit(_statementNodes);
    }

    private void ParseNumericLiteralToken(NumericLiteralToken token)
    {
        var literalNumericExpressionNode = new NumericLiteralExpressionNode(
            token);
        
        if (!_nodeStack.Any())
        {
            _nodeStack.Push(literalNumericExpressionNode);
            return;
        }
        
        var poppedNode = _nodeStack.Pop();

        switch (poppedNode.SyntaxKind)
        {
            case SyntaxKind.NumericThreePartExpressionNode:
            {
                var numericThreePartExpressionNode = (NumericThreePartExpressionNode)poppedNode;

                numericThreePartExpressionNode = new NumericThreePartExpressionNode(
                    numericThreePartExpressionNode.LeftNumericExpressionNode,
                    numericThreePartExpressionNode.OperatorNode,
                    literalNumericExpressionNode);

                _nodeStack.Push(numericThreePartExpressionNode);

                return;
            }
            case SyntaxKind.VariableAssignmentExpressionNode:
            {
                var variableAssignmentExpressionNode = (VariableAssignmentExpressionNode)poppedNode;
                var typedLiteralExpressionNode = new TypedLiteralExpressionNode(token);

                variableAssignmentExpressionNode.RightOperand = typedLiteralExpressionNode;
                _nodeStack.Push(variableAssignmentExpressionNode);
                return;
            }
            case SyntaxKind.ParenthesizedExpressionNode:
                throw new NotImplementedException();
            default:
                // TODO: Report a diagnostic and return?
                throw new NotImplementedException();
        }
    }
    
    private void ParsePlusToken(PlusToken token)
    {
        var operatorAdditionNode = new OperatorAdditionNode(
            token);
        
        if (!_nodeStack.Any())
        {
            _nodeStack.Push(operatorAdditionNode);
            return;
        }
        
        var poppedNode = _nodeStack.Pop();

        switch (poppedNode.SyntaxKind)
        {
            case SyntaxKind.NumericThreePartExpressionNode:
            {
                var numericThreePartExpressionNode = (NumericThreePartExpressionNode)poppedNode;

                numericThreePartExpressionNode = new NumericThreePartExpressionNode(
                    numericThreePartExpressionNode.LeftNumericExpressionNode,
                    operatorAdditionNode,
                    numericThreePartExpressionNode.RightNumericExpressionNode);

                _nodeStack.Push(numericThreePartExpressionNode);

                return;
            }
            default:
            {
                if (poppedNode is NumericExpressionNode numericExpressionNode)
                {
                    var numericThreePartExpressionNode = new NumericThreePartExpressionNode(
                        numericExpressionNode,
                        operatorAdditionNode,
                        null);
                    
                    _nodeStack.Push(numericThreePartExpressionNode);

                    return;
                }

                // TODO: Report a diagnostic and return?
                throw new NotImplementedException();
            }
        }
    }
    
    private void ParseKeywordToken(KeywordToken token)
    {
        if (_nodeStack.Any())
        {
            throw new ApplicationException(
                "Only keywords which start a statement are currently implemented.");
        }
        
        var keywordText = token.BlazorStudioTextSpan.GetText(_sourceText);
        
        if (_typeMap.TryGetValue(keywordText, out var typeSymbol))
        {
            // Was a primitive type such as "int"
            // which is treated as a keyword, but maps to a type.

            var typeNode = new TypeNode(
                token,
                typeSymbol
            );
            
            _nodeStack.Push(typeNode);
        }
    }
    
    private void ParseIdentifierToken(IdentifierToken token)
    {
        if (_nodeStack.Count == 0)
            return;
        
        var poppedNode = _nodeStack.Pop();

        switch (poppedNode.SyntaxKind)
        {
            case SyntaxKind.TypeNode:
            {
                var typeNode = (TypeNode)poppedNode;
            
                var variable = new Variable(typeNode.TypeSymbol, token);

                _variableMap[token.BlazorStudioTextSpan.GetText(_sourceText)] =
                    variable;

                var nextTokenByPredicate = FindNextTokenByPredicate(st => st.SyntaxKind != SyntaxKind.TriviaToken);
            
                if (nextTokenByPredicate is not null &&
                    nextTokenByPredicate.Value.token.SyntaxKind == SyntaxKind.EqualsToken)
                {
                    // Presume a definition statement like "int x = 2;"
                    _nodeStack.Push(new VariableNode(variable));
                }

                break;
            }
        }
    }
    
    private void ParseEqualsToken(EqualsToken token)
    {
        var poppedNode = _nodeStack.Pop();
        
        switch (poppedNode.SyntaxKind)
        {
            case SyntaxKind.VariableNode:
            {
                var variableNode = (VariableNode)poppedNode;

                var operatorAssignmentNode = new OperatorAssignmentNode(
                    token);

                var assignmentExpressionNode = new VariableAssignmentExpressionNode(
                    variableNode,
                    operatorAssignmentNode,
                    null,
                    typeof(object));

                _nodeStack.Push(assignmentExpressionNode);
                
                return;
            }
        }
    }
    
    private void ParseStatementDelimiterToken(StatementDelimiterToken token)
    {
        if (_nodeStack.Count == 0)
            return;
        
        var poppedNode = _nodeStack.Pop();
        var statementNode = new StatementNode(poppedNode);
        
        _statementNodes.Add(statementNode);
    }

    private (ISyntaxToken token, int index)? FindNextTokenByPredicate(Func<ISyntaxToken, bool> tokenPredicateFunc)
    {
        for (var index = _tokenIndex + 1; index < _tokens.Length; index++)
        {
            var token = _tokens[index];

            if (tokenPredicateFunc.Invoke(token))
                return (token, index);
        }

        return null;
    }
}