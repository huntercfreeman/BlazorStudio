using BlazorStudio.ClassLib.Parsing.C.SyntaxNodes.Expression;
using BlazorStudio.ClassLib.Parsing.C.BoundNodes.Expression;
using BlazorStudio.ClassLib.Parsing.C.SyntaxTokens;
using BlazorStudio.ClassLib.Parsing.C.BoundNodes;
using BlazorStudio.ClassLib.Parsing.C.Scope;

namespace BlazorStudio.ClassLib.Parsing.C;

public class Binder
{
    private readonly BoundScope _globalScope = CLanguageFacts.Scope.GetInitialGlobalScope();
    private BoundScope _currentScope;
    private readonly string _sourceText;

    public Binder(
        string sourceText)
    {
        _sourceText = sourceText;
        _currentScope = _globalScope;
    }

    public BoundLiteralExpressionNode BindLiteralExpressionNode(
        LiteralExpressionNode literalExpressionNode)
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

        if (_currentScope.TypeMap.TryGetValue(text, out var type))
        {
            boundTypeNode = new BoundTypeNode(type, token);
            return true;
        }

        boundTypeNode = null;
        return false;
    }

    public BoundFunctionDeclarationNode BindFunctionDeclarationNode(
        BoundTypeNode boundTypeNode,
        IdentifierToken identifierToken)
    {
        var text = identifierToken.BlazorStudioTextSpan.GetText(_sourceText);

        if (_currentScope.FunctionDeclarationMap.TryGetValue(
            text, 
            out var functionDeclarationNode))
        {
            // TODO: The function was already declared, so report a diagnostic?
            // TODO: The function was already declared, so check that the return types match?
            return functionDeclarationNode;
        }

        var boundFunctionDeclarationNode = new BoundFunctionDeclarationNode(
            boundTypeNode,
            identifierToken);

        _currentScope.FunctionDeclarationMap.Add(
            text,
            boundFunctionDeclarationNode);

        return boundFunctionDeclarationNode;
    }
    
    public BoundVariableDeclarationStatementNode BindVariableDeclarationNode(
        BoundTypeNode boundTypeNode,
        IdentifierToken identifierToken)
    {
        var text = identifierToken.BlazorStudioTextSpan.GetText(_sourceText);

        if (_currentScope.VariableDeclarationMap.TryGetValue(
            text, 
            out var variableDeclarationNode))
        {
            // TODO: The variable was already declared, so report a diagnostic?
            // TODO: The variable was already declared, so check that the return types match?
            return variableDeclarationNode;
        }

        var boundVariableDeclarationStatementNode = new BoundVariableDeclarationStatementNode(
            boundTypeNode,
            identifierToken);

        _currentScope.VariableDeclarationMap.Add(
            text,
            boundVariableDeclarationStatementNode);

        return boundVariableDeclarationStatementNode;
    }
    
    /// <summary>Returns null if the variable was not yet declared.</summary>
    public BoundVariableAssignmentStatementNode? BindVariableAssignmentNode(
        IdentifierToken identifierToken,
        IExpressionNode rightHandExpression)
    {
        var text = identifierToken.BlazorStudioTextSpan.GetText(_sourceText);

        if (TryGetVariableHierarchically(
                text,
                out var variableDeclarationNode) &&
            variableDeclarationNode is not null)
        {
            if (variableDeclarationNode.IsInitialized)
                return new(identifierToken, rightHandExpression);

            variableDeclarationNode = variableDeclarationNode
                .WithIsInitialized(true);

            _currentScope.VariableDeclarationMap[text] =
                variableDeclarationNode;

            return new(identifierToken, rightHandExpression);
        }
        else
        {
            // TODO: The variable was not yet declared, so report a diagnostic?
            return null;
        }
    }

    public BoundFunctionInvocationNode? BindFunctionInvocationNode(
        IdentifierToken identifierToken)
    {
        var text = identifierToken.BlazorStudioTextSpan.GetText(_sourceText);

        if (TryGetBoundFunctionDeclarationNodeHierarchically(
                text,
                out var boundFunctionDeclarationNode) &&
            boundFunctionDeclarationNode is not null)
        {
            return new(identifierToken);
        }
        else
        {
            // TODO: The function was not yet declared, so report a diagnostic?
            return null;
        }
    }

    public void RegisterBoundScope()
    {
        var functionScope = new BoundScope(
            _currentScope,
            new(),
            new(),
            new());

        _currentScope = functionScope;
    }
    
    public void DisposeBoundScope()
    {
        if (_currentScope.Parent is not null)
            _currentScope = _currentScope.Parent;
    }

    /// <summary>Search hierarchically through all the scopes, starting at the <see cref="_currentScope"/>.<br/><br/>If a match is found, then set the out parameter to it and return true.<br/><br/>If none of the searched scopes contained a match then set the out parameter to null and return false.</summary>
    public bool TryGetBoundFunctionDeclarationNodeHierarchically(
        string text,
        out BoundFunctionDeclarationNode? boundFunctionDeclarationNode)
    {
        var localScope = _currentScope;

        while (localScope is not null)
        {
            if (localScope.FunctionDeclarationMap.TryGetValue(
                    text,
                    out boundFunctionDeclarationNode))
            {
                return true;
            }

            localScope = localScope.Parent;
        }

        boundFunctionDeclarationNode = null;
        return false;
    }

    /// <summary>Search hierarchically through all the scopes, starting at the <see cref="_currentScope"/>.<br/><br/>If a match is found, then set the out parameter to it and return true.<br/><br/>If none of the searched scopes contained a match then set the out parameter to null and return false.</summary>
    public bool TryGetTypeHierarchically(
        string text,
        out Type? type)
    {
        var localScope = _currentScope;

        while (localScope is not null)
        {
            if (localScope.TypeMap.TryGetValue(
                    text,
                    out type))
            {
                return true;
            }

            localScope = localScope.Parent;
        }

        type = null;
        return false;
    }

    /// <summary>Search hierarchically through all the scopes, starting at the <see cref="_currentScope"/>.<br/><br/>If a match is found, then set the out parameter to it and return true.<br/><br/>If none of the searched scopes contained a match then set the out parameter to null and return false.</summary>
    public bool TryGetVariableHierarchically(
        string text,
        out BoundVariableDeclarationStatementNode? boundVariableDeclarationStatementNode)
    {
        var localScope = _currentScope;

        while (localScope is not null)
        {
            if (localScope.VariableDeclarationMap.TryGetValue(
                    text,
                    out boundVariableDeclarationStatementNode))
            {
                return true;
            }

            localScope = localScope.Parent;
        }

        boundVariableDeclarationStatementNode = null;
        return false;
    }
}
