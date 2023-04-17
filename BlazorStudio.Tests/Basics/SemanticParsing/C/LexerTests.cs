using BlazorStudio.ClassLib.Parsing.C;

namespace BlazorStudio.Tests.Basics.SemanticParsing.C;

public class LexerTests
{
    [Fact]
    public async Task SHOULD_LEX_PREPROCESSOR_DIRECTIVES()
    {
        string testDataHelloWorld = @"#include <stdlib.h>
#include <stdio.h>

// C:\Users\hunte\Repos\Aaa\

int main()
{
	int x = 42;

	/*
		A Multi-Line Comment
	*/
	
	/* Another Multi-Line Comment */

	printf(""%d"", x);
}".ReplaceLineEndings("\n");

        var lexer = new Lexer(testDataHelloWorld);
        
        lexer.Lex();
        
        Assert.Single(lexer.SyntaxTokens);
        
        var text = lexer.SyntaxTokens
	        .Single().BlazorStudioTextSpan
	        .GetText(testDataHelloWorld);

        Assert.Equal("42", text);

        var z = 2;
    }
}