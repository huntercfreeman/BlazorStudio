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
            lexer.SyntaxTokens);

        var compilationUnit = parser.Parse();

        var z = 2;
    }
}