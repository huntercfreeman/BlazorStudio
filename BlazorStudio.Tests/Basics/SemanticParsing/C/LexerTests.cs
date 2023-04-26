using BlazorStudio.ClassLib.Parsing.C;
using BlazorStudio.ClassLib.Parsing.C.SyntaxTokens;

namespace BlazorStudio.Tests.Basics.SemanticParsing.C;

public class LexerTests
{
    [Fact]
    public void SHOULD_LEX_NUMERIC_LITERAL_TOKENS()
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
        
        var numericLiteralToken = lexer.SyntaxTokens
	        .Single(x => x.SyntaxKind == SyntaxKind.NumericLiteralToken);
        
        var text = numericLiteralToken.BlazorStudioTextSpan
	        .GetText(testDataHelloWorld);

        Assert.Equal("42", text);
    }
    
    [Fact]
    public void SHOULD_LEX_STRING_LITERAL_TOKENS()
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
        
        var stringLiteralToken = lexer.SyntaxTokens
	        .Single(x => x.SyntaxKind == SyntaxKind.StringLiteralToken);
        
        var text = stringLiteralToken.BlazorStudioTextSpan
	        .GetText(testDataHelloWorld);

        Assert.Equal("\"%d\"", text);
    }
    
    [Fact]
    public void SHOULD_LEX_COMMENT_SINGLE_LINE_TOKENS()
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

	    var commentSingleLineTokenToken = lexer.SyntaxTokens
		    .Single(x => x.SyntaxKind == SyntaxKind.CommentSingleLineToken);

	    var tokenTextTuple = SyntaxTokenHelper.GetTokenTextTuple(
		    commentSingleLineTokenToken,
		    testDataHelloWorld);

	    Assert.Equal(
		    "// C:\\Users\\hunte\\Repos\\Aaa\\",
		    tokenTextTuple.text);
    }
    
    [Fact]
    public void SHOULD_LEX_COMMENT_MULTI_LINE_TOKENS()
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
        
	    var commentMultiLineTokenTokens = lexer.SyntaxTokens
		    .Where(x => x.SyntaxKind == SyntaxKind.CommentMultiLineToken)
		    .ToArray();

	    var tokenTextTuples = SyntaxTokenHelper.GetTokenTextTuples(
		    commentMultiLineTokenTokens,
		    testDataHelloWorld);
	
	    Assert.Equal(
		    "/*\n\t\tA Multi-Line Comment\n\t*/",
		    tokenTextTuples[0].text);
	    
	    Assert.Equal(
		    "/* Another Multi-Line Comment */",
		    tokenTextTuples[1].text);
    }
    
    [Fact]
    public void SHOULD_LEX_KEYWORD_TOKENS()
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
        
        var keywordTokens = lexer.SyntaxTokens
	        .Where(x => x.SyntaxKind == SyntaxKind.KeywordToken);
        
        var tokenTextTuples = SyntaxTokenHelper.GetTokenTextTuples(
	        keywordTokens,
	        testDataHelloWorld);
        
        Assert.Equal(
	        "int",
	        tokenTextTuples[0].text);
	    
        Assert.Equal(
	        "int",
	        tokenTextTuples[1].text);
    }
    
    [Fact]
    public void SHOULD_LEX_IDENTIFIER_TOKENS()
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
        
	    var identifierTokens = lexer.SyntaxTokens
		    .Where(x => x.SyntaxKind == SyntaxKind.IdentifierToken);
        
	    var tokenTextTuples = SyntaxTokenHelper.GetTokenTextTuples(
		    identifierTokens,
		    testDataHelloWorld);
	    
	    Assert.Equal(
		    "main",
		    tokenTextTuples[0].text);
	    
	    Assert.Equal(
		    "x",
		    tokenTextTuples[1].text);
	    
	    Assert.Equal(
		    "printf",
		    tokenTextTuples[2].text);
	    
	    Assert.Equal(
		    "x",
		    tokenTextTuples[3].text);
    }
    
    [Fact]
    public void SHOULD_LEX_PLUS_TOKENS()
    {
	    string testDataHelloWorld = @"#include <stdlib.h>
#include <stdio.h>

// C:\Users\hunte\Repos\Aaa\

int main()
{
	int x = 10 + 32;

	/*
		A Multi-Line Comment
	*/
	
	/* Another Multi-Line Comment */

	printf(""%d"", x);
}".ReplaceLineEndings("\n");

	    var lexer = new Lexer(testDataHelloWorld);
        
	    lexer.Lex();
        
	    var plusToken = lexer.SyntaxTokens
		    .Single(x => x.SyntaxKind == SyntaxKind.PlusToken);
        
	    var tokenTextTuple = SyntaxTokenHelper.GetTokenTextTuple(
		    plusToken,
		    testDataHelloWorld);
	    
	    Assert.Equal(
		    "+",
		    tokenTextTuple.text);
    }
    
    [Fact]
    public void SHOULD_LEX_PREPROCESSOR_DIRECTIVE_TOKENS()
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
        
	    var preprocessorDirectiveTokens = lexer.SyntaxTokens
		    .Where(x => x.SyntaxKind == SyntaxKind.PreprocessorDirectiveToken);
        
	    var tokenTextTuples = SyntaxTokenHelper.GetTokenTextTuples(
		    preprocessorDirectiveTokens,
		    testDataHelloWorld);

	    Assert.Equal(
		    CLanguageFacts.Preprocessor.Directives.INCLUDE,
		    tokenTextTuples[0].text);
	    
	    Assert.Equal(
		    CLanguageFacts.Preprocessor.Directives.INCLUDE,
		    tokenTextTuples[1].text);
    }
     
    [Fact]
    public void SHOULD_LEX_LIBRARY_REFERENCE_TOKENS()
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
        
	    var libraryReferenceTokens = lexer.SyntaxTokens
		    .Where(x => x.SyntaxKind == SyntaxKind.LibraryReferenceToken);
        
	    var tokenTextTuples = SyntaxTokenHelper.GetTokenTextTuples(
		    libraryReferenceTokens,
		    testDataHelloWorld);
	    
	    Assert.Equal(
		    "stdlib.h",
		    tokenTextTuples[0].text);
	    
	    Assert.Equal(
		    "stdio.h",
		    tokenTextTuples[1].text);
    }
}