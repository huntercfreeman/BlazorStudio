using BlazorStudio.ClassLib.Parsing.C;
using BlazorStudio.ClassLib.Parsing.C.BoundNodes.Expression;
using BlazorStudio.ClassLib.Parsing.C.SyntaxTokens;

namespace BlazorStudio.Tests.Basics.SemanticParsing.C;

public class ParserTests
{
    [Fact]
    public void SHOULD_PARSE_NUMERIC_LITERAL_EXPRESSION()
    {
        string sourceText = "3".ReplaceLineEndings("\n");

        var lexer = new Lexer(sourceText);
        lexer.Lex();

        var parser = new Parser(
            lexer.SyntaxTokens,
            sourceText);

        var compilationUnit = parser.Parse();

        Assert.Single(compilationUnit.Children);

        var boundLiteralExpressionNode = (BoundLiteralExpressionNode)compilationUnit
            .Children[0];

        Assert.Equal(typeof(int), boundLiteralExpressionNode.ResultType);
    }
    
    [Fact]
    public void SHOULD_PARSE_STRING_LITERAL_EXPRESSION()
    {
        string sourceText = "\"123abc\"".ReplaceLineEndings("\n");

        var lexer = new Lexer(sourceText);
        lexer.Lex();

        var parser = new Parser(
            lexer.SyntaxTokens,
            sourceText);

        var compilationUnit = parser.Parse();

        Assert.Single(compilationUnit.Children);

        var boundLiteralExpressionNode = (BoundLiteralExpressionNode)compilationUnit
            .Children[0];

        Assert.Equal(typeof(string), boundLiteralExpressionNode.ResultType);
    }
    
    [Fact]
    public void SHOULD_PARSE_NUMERIC_BINARY_EXPRESSION()
    {
        string sourceText = "3 + 3".ReplaceLineEndings("\n");

        var lexer = new Lexer(sourceText);
        lexer.Lex();

        var parser = new Parser(
            lexer.SyntaxTokens,
            sourceText);

        var compilationUnit = parser.Parse();

        Assert.Single(compilationUnit.Children);

        var boundBinaryExpressionNode = (BoundBinaryExpressionNode)compilationUnit
            .Children[0];

        Assert.Equal(
            typeof(int),
            boundBinaryExpressionNode.LeftBoundExpressionNode.ResultType);

        Assert.Equal(
            typeof(int),
            boundBinaryExpressionNode.BoundBinaryOperatorNode.ResultType);

        Assert.Equal(
            typeof(int),
            boundBinaryExpressionNode.RightBoundExpressionNode.ResultType);
    }

    [Fact]
    public void SHOULD_PARSE_LIBRARY_REFERENCE()
    {
        string sourceText = "#include <stdlib.h>"
            .ReplaceLineEndings("\n");

        var lexer = new Lexer(sourceText);

        lexer.Lex();

        var parser = new Parser(
            lexer.SyntaxTokens,
            sourceText);

        var compilationUnit = parser.Parse();
    }

    [Fact]
    public void SHOULD_PARSE_TWO_LIBRARY_REFERENCES()
    {
        string sourceText = @"#include <stdlib.h>
#include <stdio.h>"
            .ReplaceLineEndings("\n");

        var lexer = new Lexer(sourceText);

        lexer.Lex();

        var parser = new Parser(
            lexer.SyntaxTokens,
            sourceText);

        var compilationUnit = parser.Parse();
    }
    
    [Fact]
    public void SHOULD_NOT_PARSE_COMMENT_SINGLE_LINE_STATEMENT()
    {
        string sourceText = @"// C:\Users\hunte\Repos\Aaa\"
            .ReplaceLineEndings("\n");

        var lexer = new Lexer(sourceText);

        lexer.Lex();

        var parser = new Parser(
            lexer.SyntaxTokens,
            sourceText);

        var compilationUnit = parser.Parse();

        Assert.Empty(compilationUnit.Children);
    }

    [Fact]
    public void SHOULD_PARSE_FUNCTION_DEFINITION_STATEMENT()
    {
        string sourceText = @"int main()
{
	int x = 42;

	/*
		A Multi-Line Comment
	*/
	
	/* Another Multi-Line Comment */

	printf(""%d"", x);
}"
            .ReplaceLineEndings("\n");

        var lexer = new Lexer(sourceText);

        lexer.Lex();

        var parser = new Parser(
            lexer.SyntaxTokens,
            sourceText);

        var compilationUnit = parser.Parse();

        Assert.Empty(compilationUnit.Children);
    }
}