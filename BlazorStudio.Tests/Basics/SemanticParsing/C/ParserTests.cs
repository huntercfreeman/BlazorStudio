using BlazorStudio.ClassLib.Parsing.C;

namespace BlazorStudio.Tests.Basics.SemanticParsing.C;

public class ParserTests
{
    [Fact]
    public void SHOULD_PARSE_NUMERIC_LITERAL_EXPRESSION()
    {
        string testDataHelloWorld = "3".ReplaceLineEndings("\n");

        var lexer = new Lexer(testDataHelloWorld);
        lexer.Lex();

        var parser = new Parser(lexer.SyntaxTokens);
        
        var root = parser.Parse();
    }
    
    [Fact]
    public void SHOULD_PARSE_NUMERIC_THREE_PART_EXPRESSION()
    {
        string testDataHelloWorld = "10 + 32".ReplaceLineEndings("\n");

        var lexer = new Lexer(testDataHelloWorld);
        lexer.Lex();

        var parser = new Parser(lexer.SyntaxTokens);
        
        var root = parser.Parse();
    }
}