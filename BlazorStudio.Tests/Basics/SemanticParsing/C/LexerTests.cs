using BlazorStudio.ClassLib.Parsing.C;
using BlazorStudio.ClassLib.Parsing.C.SyntaxTokens;

namespace BlazorStudio.Tests.Basics.SemanticParsing.C;

public class LexerTests
{
    [Fact]
    public void SHOULD_LEX_NUMERIC_LITERAL_TOKENS()
    {
        string testData = @"#include <stdlib.h>
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

        var lexer = new Lexer(testData);
        
        lexer.Lex();
        
        var numericLiteralToken = lexer.SyntaxTokens
	        .Single(x => x.SyntaxKind == SyntaxKind.NumericLiteralToken);
        
        var text = numericLiteralToken.BlazorStudioTextSpan
	        .GetText(testData);

        Assert.Equal("42", text);
    }
    
    [Fact]
    public void SHOULD_LEX_STRING_LITERAL_TOKENS()
    {
        string testData = @"#include <stdlib.h>
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

        var lexer = new Lexer(testData);
        
        lexer.Lex();
        
        var stringLiteralToken = lexer.SyntaxTokens
	        .Single(x => x.SyntaxKind == SyntaxKind.StringLiteralToken);
        
        var text = stringLiteralToken.BlazorStudioTextSpan
	        .GetText(testData);

        Assert.Equal("\"%d\"", text);
    }
    
    [Fact]
    public void SHOULD_LEX_COMMENT_SINGLE_LINE_TOKENS()
    {
	    string testData = @"#include <stdlib.h>
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

	    var lexer = new Lexer(testData);
        
	    lexer.Lex();

	    var commentSingleLineTokenToken = lexer.SyntaxTokens
		    .Single(x => x.SyntaxKind == SyntaxKind.CommentSingleLineToken);

	    var tokenTextTuple = SyntaxTokenHelper.GetTokenTextTuple(
		    commentSingleLineTokenToken,
		    testData);

	    Assert.Equal(
		    "// C:\\Users\\hunte\\Repos\\Aaa\\",
		    tokenTextTuple.text);
    }
    
    [Fact]
    public void SHOULD_LEX_COMMENT_MULTI_LINE_TOKENS()
    {
	    string testData = @"#include <stdlib.h>
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

	    var lexer = new Lexer(testData);
        
	    lexer.Lex();
        
	    var commentMultiLineTokenTokens = lexer.SyntaxTokens
		    .Where(x => x.SyntaxKind == SyntaxKind.CommentMultiLineToken)
		    .ToArray();

	    var tokenTextTuples = SyntaxTokenHelper.GetTokenTextTuples(
		    commentMultiLineTokenTokens,
		    testData);
	
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
        string testData = @"#include <stdlib.h>
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

        var lexer = new Lexer(testData);
        
        lexer.Lex();
        
        var keywordTokens = lexer.SyntaxTokens
	        .Where(x => x.SyntaxKind == SyntaxKind.KeywordToken);
        
        var tokenTextTuples = SyntaxTokenHelper.GetTokenTextTuples(
	        keywordTokens,
	        testData);
        
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
	    string testData = @"#include <stdlib.h>
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

	    var lexer = new Lexer(testData);
        
	    lexer.Lex();
        
	    var identifierTokens = lexer.SyntaxTokens
		    .Where(x => x.SyntaxKind == SyntaxKind.IdentifierToken);
        
	    var tokenTextTuples = SyntaxTokenHelper.GetTokenTextTuples(
		    identifierTokens,
		    testData);
	    
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
	    string testData = @"#include <stdlib.h>
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

	    var lexer = new Lexer(testData);
        
	    lexer.Lex();
        
	    var plusToken = lexer.SyntaxTokens
		    .Single(x => x.SyntaxKind == SyntaxKind.PlusToken);
        
	    var tokenTextTuple = SyntaxTokenHelper.GetTokenTextTuple(
		    plusToken,
		    testData);
	    
	    Assert.Equal(
		    "+",
		    tokenTextTuple.text);
    }
    
    [Fact]
    public void SHOULD_LEX_PREPROCESSOR_DIRECTIVE_TOKENS()
    {
	    string testData = @"#include <stdlib.h>
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

	    var lexer = new Lexer(testData);
        
	    lexer.Lex();
        
	    var preprocessorDirectiveTokens = lexer.SyntaxTokens
		    .Where(x => x.SyntaxKind == SyntaxKind.PreprocessorDirectiveToken);
        
	    var tokenTextTuples = SyntaxTokenHelper.GetTokenTextTuples(
		    preprocessorDirectiveTokens,
		    testData);

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
	    string testData = @"#include <stdlib.h>
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

	    var lexer = new Lexer(testData);
        
	    lexer.Lex();
        
	    var libraryReferenceTokens = lexer.SyntaxTokens
		    .Where(x => x.SyntaxKind == SyntaxKind.LibraryReferenceToken);
        
	    var tokenTextTuples = SyntaxTokenHelper.GetTokenTextTuples(
		    libraryReferenceTokens,
		    testData);
	    
	    Assert.Equal(
		    "stdlib.h",
		    tokenTextTuples[0].text);
	    
	    Assert.Equal(
		    "stdio.h",
		    tokenTextTuples[1].text);
    }
    
    [Fact]
    public void SHOULD_LEX_EQUALS_TOKENS()
    {
	    string testData = @"#include <stdlib.h>
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

	    var lexer = new Lexer(testData);
        
	    lexer.Lex();
        
	    var equalsTokens = lexer.SyntaxTokens
		    .Single(x => x.SyntaxKind == SyntaxKind.EqualsToken);
        
	    var tokenTextTuple = SyntaxTokenHelper.GetTokenTextTuple(
		    equalsTokens,
		    testData);
	    
	    Assert.Equal(
		    "=",
		    tokenTextTuple.text);
    }
    
    [Fact]
    public void SHOULD_LEX_STATEMENT_DELIMITER_TOKENS()
    {
	    string testData = @"#include <stdlib.h>
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

	    var lexer = new Lexer(testData);
        
	    lexer.Lex();
        
	    var statementDelimiterTokens = lexer.SyntaxTokens
		    .Where(x => x.SyntaxKind == SyntaxKind.StatementDelimiterToken);
        
	    var tokenTextTuples = SyntaxTokenHelper.GetTokenTextTuples(
		    statementDelimiterTokens,
		    testData);
	    
	    Assert.Equal(2, tokenTextTuples.Length);
	    
	    Assert.Equal(
		    ";",
		    tokenTextTuples[0].text);
	    
	    Assert.Equal(
		    ";",
		    tokenTextTuples[1].text);
    }
}