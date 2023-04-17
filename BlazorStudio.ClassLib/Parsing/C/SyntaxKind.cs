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
    
    // Nodes
    NumericLiteralExpressionNode,
    NumericThreePartExpressionNode,
    VariableAssignmentExpressionNode,
    ParenthesizedExpressionNode,
    OperatorAdditionNode
}