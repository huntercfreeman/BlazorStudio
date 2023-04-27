namespace BlazorStudio.ClassLib.Parsing.C;

public enum SyntaxKind
{
    // Tokens
    CommentMultiLineToken,
    CommentSingleLineToken,
    IdentifierToken,
    KeywordToken,
    NumericLiteralToken,
    StringLiteralToken,
    TriviaToken,
    PreprocessorDirectiveToken,
    LibraryReferenceToken,
    PlusToken,
    EqualsToken,
    StatementDelimiterToken,
    EndOfFileToken,
    
    // Nodes
    NumericLiteralExpressionNode,
    NumericThreePartExpressionNode,
    VariableAssignmentExpressionNode,
    ParenthesizedExpressionNode,
    OperatorAdditionNode,
    PreprocessorDirectiveNode,
    LibraryReferenceNode,
    TypeNode,
    StatementNode,
    VariableNode,
    TypedLiteralExpressionNode,
    TypedUnaryExpressionNode,
    TypedBinaryExpressionNode,
    
    // Symbols
    IntTypeSymbol,
    
    // Variable
    Variable,
}