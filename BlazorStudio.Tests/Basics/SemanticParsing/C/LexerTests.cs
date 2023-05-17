using BlazorStudio.ClassLib.Parsing.C;
using BlazorStudio.ClassLib.Parsing.C.SyntaxTokens;

namespace BlazorStudio.Tests.Basics.SemanticParsing.C;

public class LexerTests
{
    [Fact]
    public void SHOULD_LEX_NUMERIC_LITERAL_TOKENS()
    {
        string sourceText = @"#include <stdlib.h>
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

        var lexer = new Lexer(sourceText);
        
        lexer.Lex();
        
        var numericLiteralToken = lexer.SyntaxTokens
	        .Single(x => x.SyntaxKind == SyntaxKind.NumericLiteralToken);
        
        var text = numericLiteralToken.BlazorStudioTextSpan
	        .GetText(sourceText);

        Assert.Equal("42", text);
    }
    
    [Fact]
    public void SHOULD_LEX_STRING_LITERAL_TOKENS()
    {
        string sourceText = @"#include <stdlib.h>
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

        var lexer = new Lexer(sourceText);
        
        lexer.Lex();
        
        var stringLiteralToken = lexer.SyntaxTokens
	        .Single(x => x.SyntaxKind == SyntaxKind.StringLiteralToken);
        
        var text = stringLiteralToken.BlazorStudioTextSpan
	        .GetText(sourceText);

        Assert.Equal("\"%d\"", text);
    }
    
    [Fact]
    public void SHOULD_LEX_COMMENT_SINGLE_LINE_TOKENS()
    {
	    string sourceText = @"#include <stdlib.h>
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

	    var lexer = new Lexer(sourceText);
        
	    lexer.Lex();

	    var commentSingleLineTokenToken = lexer.SyntaxTokens
		    .Single(x => x.SyntaxKind == SyntaxKind.CommentSingleLineToken);

	    var tokenTextTuple = SyntaxTokenHelper.GetTokenTextTuple(
		    commentSingleLineTokenToken,
		    sourceText);

	    Assert.Equal(
		    "// C:\\Users\\hunte\\Repos\\Aaa\\",
		    tokenTextTuple.text);
    }
    
    [Fact]
    public void SHOULD_LEX_COMMENT_MULTI_LINE_TOKENS()
    {
	    string sourceText = @"#include <stdlib.h>
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

	    var lexer = new Lexer(sourceText);
        
	    lexer.Lex();
        
	    var commentMultiLineTokenTokens = lexer.SyntaxTokens
		    .Where(x => x.SyntaxKind == SyntaxKind.CommentMultiLineToken)
		    .ToArray();

	    var tokenTextTuples = SyntaxTokenHelper.GetTokenTextTuples(
		    commentMultiLineTokenTokens,
		    sourceText);
	
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
        string sourceText = @"#include <stdlib.h>
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

        var lexer = new Lexer(sourceText);
        
        lexer.Lex();
        
        var keywordTokens = lexer.SyntaxTokens
	        .Where(x => x.SyntaxKind == SyntaxKind.KeywordToken);
        
        var tokenTextTuples = SyntaxTokenHelper.GetTokenTextTuples(
	        keywordTokens,
	        sourceText);
        
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
	    string sourceText = @"#include <stdlib.h>
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

	    var lexer = new Lexer(sourceText);
        
	    lexer.Lex();
        
	    var identifierTokens = lexer.SyntaxTokens
		    .Where(x => x.SyntaxKind == SyntaxKind.IdentifierToken);
        
	    var tokenTextTuples = SyntaxTokenHelper.GetTokenTextTuples(
		    identifierTokens,
		    sourceText);
	    
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
	    string sourceText = @"#include <stdlib.h>
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

	    var lexer = new Lexer(sourceText);
        
	    lexer.Lex();
        
	    var plusToken = lexer.SyntaxTokens
		    .Single(x => x.SyntaxKind == SyntaxKind.PlusToken);
        
	    var tokenTextTuple = SyntaxTokenHelper.GetTokenTextTuple(
		    plusToken,
		    sourceText);
	    
	    Assert.Equal(
		    "+",
		    tokenTextTuple.text);
    }
    
    [Fact]
    public void SHOULD_LEX_PREPROCESSOR_DIRECTIVE_TOKENS()
    {
	    string sourceText = @"#include <stdlib.h>
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

	    var lexer = new Lexer(sourceText);
        
	    lexer.Lex();
        
	    var preprocessorDirectiveTokens = lexer.SyntaxTokens
		    .Where(x => x.SyntaxKind == SyntaxKind.PreprocessorDirectiveToken);
        
	    var tokenTextTuples = SyntaxTokenHelper.GetTokenTextTuples(
		    preprocessorDirectiveTokens,
		    sourceText);

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
	    string sourceText = @"#include <stdlib.h>
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

	    var lexer = new Lexer(sourceText);
        
	    lexer.Lex();
        
	    var libraryReferenceTokens = lexer.SyntaxTokens
		    .Where(x => x.SyntaxKind == SyntaxKind.LibraryReferenceToken);
        
	    var tokenTextTuples = SyntaxTokenHelper.GetTokenTextTuples(
		    libraryReferenceTokens,
		    sourceText);
	    
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
	    string sourceText = @"#include <stdlib.h>
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

	    var lexer = new Lexer(sourceText);
        
	    lexer.Lex();
        
	    var equalsTokens = lexer.SyntaxTokens
		    .Single(x => x.SyntaxKind == SyntaxKind.EqualsToken);
        
	    var tokenTextTuple = SyntaxTokenHelper.GetTokenTextTuple(
		    equalsTokens,
		    sourceText);
	    
	    Assert.Equal(
		    "=",
		    tokenTextTuple.text);
    }
    
    [Fact]
    public void SHOULD_LEX_STATEMENT_DELIMITER_TOKENS()
    {
	    string sourceText = @"#include <stdlib.h>
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

	    var lexer = new Lexer(sourceText);
        
	    lexer.Lex();
        
	    var statementDelimiterTokens = lexer.SyntaxTokens
		    .Where(x => x.SyntaxKind == SyntaxKind.StatementDelimiterToken);
        
	    var tokenTextTuples = SyntaxTokenHelper.GetTokenTextTuples(
		    statementDelimiterTokens,
		    sourceText);
	    
	    Assert.Equal(2, tokenTextTuples.Length);
	    
	    Assert.Equal(
		    ";",
		    tokenTextTuples[0].text);
	    
	    Assert.Equal(
		    ";",
		    tokenTextTuples[1].text);
    }
	
	[Fact]
    public void SHOULD_LEX_OPEN_PARENTHESIS_TOKENS()
    {
	    string sourceText = @"#include <stdlib.h>
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

	    var lexer = new Lexer(sourceText);
        
	    lexer.Lex();
        
	    var tokens = lexer.SyntaxTokens
		    .Where(x => x.SyntaxKind == SyntaxKind.OpenParenthesisToken);
        
	    var tokenTextTuples = SyntaxTokenHelper.GetTokenTextTuples(
		    tokens,
		    sourceText);
	    
	    Assert.Equal(2, tokenTextTuples.Length);
	    
	    Assert.Equal(
		    "(",
		    tokenTextTuples[0].text);
	    
	    Assert.Equal(
            "(",
		    tokenTextTuples[1].text);
    }
	
	[Fact]
    public void SHOULD_LEX_CLOSE_PARENTHESIS_TOKENS()
    {
	    string sourceText = @"#include <stdlib.h>
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

	    var lexer = new Lexer(sourceText);
        
	    lexer.Lex();
        
	    var tokens = lexer.SyntaxTokens
		    .Where(x => x.SyntaxKind == SyntaxKind.CloseParenthesisToken);
        
	    var tokenTextTuples = SyntaxTokenHelper.GetTokenTextTuples(
		    tokens,
		    sourceText);
	    
	    Assert.Equal(2, tokenTextTuples.Length);
	    
	    Assert.Equal(
		    ")",
		    tokenTextTuples[0].text);
	    
	    Assert.Equal(
            ")",
		    tokenTextTuples[1].text);
    }
	
	[Fact]
    public void SHOULD_LEX_OPEN_BRACE_TOKENS()
    {
	    string sourceText = @"#include <stdlib.h>
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

	    var lexer = new Lexer(sourceText);
        
	    lexer.Lex();
        
	    var token = lexer.SyntaxTokens
		    .Single(x => x.SyntaxKind == SyntaxKind.OpenBraceToken);
        
	    var tokenTextTuple = SyntaxTokenHelper.GetTokenTextTuple(
		    token,
		    sourceText);
	    
	    Assert.Equal(
		    "{",
		    tokenTextTuple.text);
    }
	
	[Fact]
    public void SHOULD_LEX_CLOSE_BRACE_TOKENS()
    {
	    string sourceText = @"#include <stdlib.h>
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

	    var lexer = new Lexer(sourceText);
        
	    lexer.Lex();
        
	    var token = lexer.SyntaxTokens
		    .Single(x => x.SyntaxKind == SyntaxKind.CloseBraceToken);
        
	    var tokenTextTuple = SyntaxTokenHelper.GetTokenTextTuple(
		    token,
		    sourceText);
	    
	    Assert.Equal(
		    "}",
		    tokenTextTuple.text);
    }
}