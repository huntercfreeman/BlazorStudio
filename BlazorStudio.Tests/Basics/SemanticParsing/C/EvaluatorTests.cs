using BlazorStudio.ClassLib.Parsing.C;

namespace BlazorStudio.Tests.Basics.SemanticParsing.C;

public class EvaluatorTests
{
    [Fact]
    public void SHOULD_EVALUATE_NUMERIC_THREE_PART_EXPRESSION()
    {
        var x = 10;
        var y = 32;
        
        string testDataHelloWorld = $"{x} + {y}".ReplaceLineEndings("\n");

        var lexer = new Lexer(testDataHelloWorld);
        lexer.Lex();

        var parser = new Parser(lexer.SyntaxTokens);
        
        var root = parser.Parse();

        var evaluator = new Evaluator(
            root,
            testDataHelloWorld);

        var evaluatorResult = evaluator.Evaluate();
        
        Assert.NotNull(evaluatorResult);

        Assert.Equal(
            x + y,
            evaluatorResult!.ResultValue);
    }
}