using BlazorStudio.ClassLib.Parsing.C;

namespace BlazorStudio.Tests.Basics.SemanticParsing.C;

public class EvaluatorTests
{
    [Fact]
    public void SHOULD_EVALUATE_NUMERIC_THREE_PART_EXPRESSION()
    {
        string testDataHelloWorld = "10 + 32".ReplaceLineEndings("\n");

        var lexer = new Lexer(testDataHelloWorld);
        lexer.Lex();

        var parser = new Parser(lexer.SyntaxTokens);
        
        var root = parser.Parse();

        var evaluator = new Evaluator(
            root,
            testDataHelloWorld);

        var evaluatorResult = evaluator.Evaluate();
    }
}