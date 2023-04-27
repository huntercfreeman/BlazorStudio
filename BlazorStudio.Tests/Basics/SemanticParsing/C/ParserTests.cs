using BlazorStudio.ClassLib.Parsing.C;

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
            lexer.SyntaxTokens,
            testData);
        
        var root = parser.Parse();
    }
    
    [Fact]
    public void SHOULD_PARSE_NUMERIC_THREE_PART_EXPRESSION()
    {
        string testData = "10 + 32".ReplaceLineEndings("\n");

        var lexer = new Lexer(testData);
        lexer.Lex();

        var parser = new Parser(
            lexer.SyntaxTokens,
            testData);
        
        var root = parser.Parse();
    }
    
    /// <summary>
    /// Variable Definition here is to mean "int x = 2;"
    /// Variable Declaration here is to mean "int x;"
    /// Variable Assignment here is to mean "x = 2;"
    /// </summary>
    [Fact]
    public void SHOULD_PARSE_VARIABLE_DEFINITION()
    {
        string testData = "int x = 42;".ReplaceLineEndings("\n");

        var lexer = new Lexer(testData);
        lexer.Lex();

        var parser = new Parser(
            lexer.SyntaxTokens,
            testData);
        
        var root = parser.Parse();
    }
}