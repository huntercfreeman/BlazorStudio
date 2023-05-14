using BlazorStudio.ClassLib.Parsing.C;
using BlazorStudio.ClassLib.Parsing.C.BoundNodes.Expression;

namespace BlazorStudio.Tests.Basics.SemanticParsing.C;

public class ParserTests
{
    [Fact]
    public void SHOULD_PARSE_NUMERIC_LITERAL_EXPRESSION()
    {
        string testData = "3".ReplaceLineEndings("\n");

        var lexer = new Lexer(testData);
        lexer.Lex();

        var parser = new Parser(
            lexer.SyntaxTokens);

        var compilationUnit = parser.Parse();

        Assert.Single(compilationUnit.Children);

        var boundLiteralExpressionNode = (BoundLiteralExpressionNode)compilationUnit
            .Children[0];

        Assert.Equal(typeof(Int32), boundLiteralExpressionNode.ResultType);
    }
    
    [Fact]
    public void SHOULD_PARSE_STRING_LITERAL_EXPRESSION()
    {
        string testData = "\"123abc\"".ReplaceLineEndings("\n");

        var lexer = new Lexer(testData);
        lexer.Lex();

        var parser = new Parser(
            lexer.SyntaxTokens);

        var compilationUnit = parser.Parse();

        Assert.Single(compilationUnit.Children);

        var boundLiteralExpressionNode = (BoundLiteralExpressionNode)compilationUnit
            .Children[0];

        Assert.Equal(typeof(string), boundLiteralExpressionNode.ResultType);
    }
    
    [Fact]
    public void SHOULD_PARSE_NUMERIC_BINARY_EXPRESSION()
    {
        string testData = "3 + 3".ReplaceLineEndings("\n");

        var lexer = new Lexer(testData);
        lexer.Lex();

        var parser = new Parser(
            lexer.SyntaxTokens);

        var compilationUnit = parser.Parse();

        Assert.Single(compilationUnit.Children);

        var boundLiteralExpressionNode = (BoundLiteralExpressionNode)compilationUnit
            .Children[0];

        Assert.Equal(typeof(Int32), boundLiteralExpressionNode.ResultType);
    }
}