using BlazorStudio.ClassLib.Parsing.C;

namespace BlazorStudio.Tests.Basics.SemanticParsing.C;

public class EvaluatorTests
{
    [Fact]
    public void SHOULD_EVALUATE_NUMERIC_THREE_PART_EXPRESSION()
    {
        var x = 10;
        var y = 32;
        
        string testData = $"{x} + {y}".ReplaceLineEndings("\n");

        var lexer = new Lexer(testData);
        lexer.Lex();

        var parser = new Parser(
            lexer.SyntaxTokens,
            testData);
        
        var root = parser.Parse();

        var evaluator = new Evaluator(
            root,
            testData);

        var evaluatorResult = evaluator.Evaluate();
        
        Assert.NotNull(evaluatorResult);

        Assert.Equal(
            x + y,
            evaluatorResult!.ResultValue);
    }
}