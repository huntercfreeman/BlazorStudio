using BlazorStudio.ClassLib.Parsing.C;

namespace BlazorStudio.Tests.Basics.SemanticParsing.C;

public class LexerTests
{
    [Fact]
    public void SHOULD_LEX_NUMERIC_LITERAL_TOKEN()
    {
		var numericValue = 4135;
        var sourceText = $"{numericValue}".ReplaceLineEndings("\n");

        var lexer = new Lexer(sourceText);
        lexer.Lex();
        
        var numericLiteralToken = lexer.SyntaxTokens.First();

		Assert.Equal(SyntaxKind.NumericLiteralToken, numericLiteralToken.SyntaxKind);

        var text = numericLiteralToken.BlazorStudioTextSpan.GetText(sourceText);
        Assert.Equal(numericValue, int.Parse(text));
    }
    
    [Fact]
    public void SHOULD_LEX_STRING_LITERAL_TOKEN()
    {
		var stringValue = "\"Apple Sauce\"";
        var sourceText = $"{stringValue}".ReplaceLineEndings("\n");

        var lexer = new Lexer(sourceText);
        lexer.Lex();
        
        var stringLiteralToken = lexer.SyntaxTokens.First();

        Assert.Equal(SyntaxKind.StringLiteralToken, stringLiteralToken.SyntaxKind);

        var text = stringLiteralToken.BlazorStudioTextSpan.GetText(sourceText);
        Assert.Equal(stringValue, text);
    }
    
    [Fact]
    public void SHOULD_LEX_COMMENT_SINGLE_LINE_TOKEN_WITH_ENDING_AS_END_OF_FILE()
    {
        var singleLineCommentAsString = @"// C:\Users\hunte\Repos\Aaa\";
        var sourceText = $"{singleLineCommentAsString}".ReplaceLineEndings("\n");

	    var lexer = new Lexer(sourceText);
	    lexer.Lex();

	    var commentSingleLineToken = lexer.SyntaxTokens.First();

        Assert.Equal(SyntaxKind.CommentSingleLineToken, commentSingleLineToken.SyntaxKind);

        var text = commentSingleLineToken.BlazorStudioTextSpan.GetText(sourceText);
        Assert.Equal(singleLineCommentAsString, text);
    }
	
	[Fact]
    public void SHOULD_LEX_COMMENT_SINGLE_LINE_TOKEN_WITH_ENDING_AS_NEW_LINE()
    {
        var singleLineCommentAsString = @"// C:\Users\hunte\Repos\Aaa\";
        var sourceText = $@"{singleLineCommentAsString}
".ReplaceLineEndings("\n");

	    var lexer = new Lexer(sourceText);
	    lexer.Lex();

	    var commentSingleLineToken = lexer.SyntaxTokens.First();

        Assert.Equal(SyntaxKind.CommentSingleLineToken, commentSingleLineToken.SyntaxKind);
        
		var text = commentSingleLineToken.BlazorStudioTextSpan.GetText(sourceText);
        Assert.Equal(singleLineCommentAsString, text);
    }
    
    [Fact]
    public void SHOULD_LEX_COMMENT_MULTI_LINE_TOKEN_WRITTEN_ON_MULTIPLE_LINES()
    {
        var multiLineCommentAsString = @"/*
	A Multi-Line Comment
*/".ReplaceLineEndings("\n");

        var sourceText = $"{multiLineCommentAsString}".ReplaceLineEndings("\n");

	    var lexer = new Lexer(sourceText);
	    lexer.Lex();
        
	    var commentMultiLineToken = lexer.SyntaxTokens.First();

        Assert.Equal(SyntaxKind.CommentMultiLineToken, commentMultiLineToken.SyntaxKind);

        var text = commentMultiLineToken.BlazorStudioTextSpan.GetText(sourceText);
        Assert.Equal(multiLineCommentAsString, text);
    }
	
	[Fact]
    public void SHOULD_LEX_COMMENT_MULTI_LINE_TOKEN_WRITTEN_ON_SINGLE_LINE()
    {
        var multiLineCommentAsString = @"/* Another Multi-Line Comment */"
			.ReplaceLineEndings("\n");

        var sourceText = $"{multiLineCommentAsString}".ReplaceLineEndings("\n");

	    var lexer = new Lexer(sourceText);
	    lexer.Lex();
        
	    var commentMultiLineToken = lexer.SyntaxTokens.First();

        Assert.Equal(SyntaxKind.CommentMultiLineToken, commentMultiLineToken.SyntaxKind);

        var text = commentMultiLineToken.BlazorStudioTextSpan.GetText(sourceText);
        Assert.Equal(multiLineCommentAsString, text);
    }
    
    [Fact]
    public void SHOULD_LEX_KEYWORD_TOKEN()
    {
		var keywordAsString = "int";
        var sourceText = $"{keywordAsString}".ReplaceLineEndings("\n");

        var lexer = new Lexer(sourceText);
        lexer.Lex();
        
        var keywordToken = lexer.SyntaxTokens.First();

        Assert.Equal(SyntaxKind.KeywordToken, keywordToken.SyntaxKind);

        var text = keywordToken.BlazorStudioTextSpan.GetText(sourceText);
        Assert.Equal(keywordAsString, text);
    }
    
    [Fact]
    public void SHOULD_LEX_IDENTIFIER_TOKEN()
    {
		var identifierAsString = "main";
        var sourceText = $"{identifierAsString}".ReplaceLineEndings("\n");

	    var lexer = new Lexer(sourceText);
	    lexer.Lex();
        
	    var identifierToken = lexer.SyntaxTokens.First();

        Assert.Equal(SyntaxKind.IdentifierToken, identifierToken.SyntaxKind);

        var text = identifierToken.BlazorStudioTextSpan.GetText(sourceText);
        Assert.Equal(identifierAsString, text);
    }
    
    [Fact]
    public void SHOULD_LEX_PLUS_TOKEN()
    {
		var plusTokenAsString = "+";
        var sourceText = $"{plusTokenAsString}".ReplaceLineEndings("\n");

	    var lexer = new Lexer(sourceText);
	    lexer.Lex();
        
	    var plusToken = lexer.SyntaxTokens.First();
        
		Assert.Equal(SyntaxKind.PlusToken, plusToken.SyntaxKind);

        var text = plusToken.BlazorStudioTextSpan.GetText(sourceText);
        Assert.Equal(plusTokenAsString, text);
    }
    
    [Fact]
    public void SHOULD_LEX_PREPROCESSOR_DIRECTIVE_TOKEN()
    {
		var preprocessorDirectiveAsString = "include";
        var sourceText = $"#{preprocessorDirectiveAsString}".ReplaceLineEndings("\n");

	    var lexer = new Lexer(sourceText);
	    lexer.Lex();
        
	    var preprocessorDirectiveToken = lexer.SyntaxTokens.First();

        Assert.Equal(SyntaxKind.PreprocessorDirectiveToken, preprocessorDirectiveToken.SyntaxKind);

        var text = preprocessorDirectiveToken.BlazorStudioTextSpan.GetText(sourceText);
        Assert.Equal(preprocessorDirectiveAsString, text);
    }
     
    [Fact]
    public void SHOULD_LEX_PREPROCESSOR_DIRECTIVE_TOKEN_AND_LIBRARY_REFERENCE_TOKEN()
    {
        var preprocessorDirectiveAsString = "include";
        var libraryReferenceAsString = "<stdlib.h>";
        var sourceText = $"#{preprocessorDirectiveAsString} {libraryReferenceAsString}".ReplaceLineEndings("\n");

        var lexer = new Lexer(sourceText);
        lexer.Lex();

        // Preprocessor Directive Token Assertions
        {
            var preprocessorDirectiveToken = lexer.SyntaxTokens[0];

            Assert.Equal(SyntaxKind.PreprocessorDirectiveToken, preprocessorDirectiveToken.SyntaxKind);

            var text = preprocessorDirectiveToken.BlazorStudioTextSpan.GetText(sourceText);
            Assert.Equal(preprocessorDirectiveAsString, text);
        }
        
        // Library Reference Token Assertions
        {
            var libraryReferenceToken = lexer.SyntaxTokens[1];

            Assert.Equal(SyntaxKind.LibraryReferenceToken, libraryReferenceToken.SyntaxKind);

            var text = libraryReferenceToken.BlazorStudioTextSpan.GetText(sourceText);
            Assert.Equal(libraryReferenceAsString, text);
        }
    }
    
    [Fact]
    public void SHOULD_LEX_EQUALS_TOKEN()
    {
        var equalsTokenAsString = "=";
        var sourceText = $"{equalsTokenAsString}".ReplaceLineEndings("\n");

        var lexer = new Lexer(sourceText);
        lexer.Lex();

        var equalsToken = lexer.SyntaxTokens.First();

        Assert.Equal(SyntaxKind.EqualsToken, equalsToken.SyntaxKind);

        var text = equalsToken.BlazorStudioTextSpan.GetText(sourceText);
        Assert.Equal(equalsTokenAsString, text);
    }
    
    [Fact]
    public void SHOULD_LEX_STATEMENT_DELIMITER_TOKEN()
    {
        var statementDelimiterTokenAsString = ";";
        var sourceText = $"{statementDelimiterTokenAsString}".ReplaceLineEndings("\n");

        var lexer = new Lexer(sourceText);
        lexer.Lex();

        var statementDelimiterToken = lexer.SyntaxTokens.First();

        Assert.Equal(SyntaxKind.StatementDelimiterToken, statementDelimiterToken.SyntaxKind);

        var text = statementDelimiterToken.BlazorStudioTextSpan.GetText(sourceText);
        Assert.Equal(statementDelimiterTokenAsString, text);
    }
	
	[Fact]
    public void SHOULD_LEX_OPEN_PARENTHESIS_TOKEN()
    {
        var openParenthesisTokenAsString = "(";
        var sourceText = $"{openParenthesisTokenAsString}".ReplaceLineEndings("\n");

        var lexer = new Lexer(sourceText);
        lexer.Lex();

        var openParenthesisToken = lexer.SyntaxTokens.First();

        Assert.Equal(SyntaxKind.OpenParenthesisToken, openParenthesisToken.SyntaxKind);

        var text = openParenthesisToken.BlazorStudioTextSpan.GetText(sourceText);
        Assert.Equal(openParenthesisTokenAsString, text);
    }
	
	[Fact]
    public void SHOULD_LEX_CLOSE_PARENTHESIS_TOKEN()
    {
        var closeParenthesisTokenAsString = ")";
        var sourceText = $"{closeParenthesisTokenAsString}".ReplaceLineEndings("\n");

        var lexer = new Lexer(sourceText);
        lexer.Lex();

        var closeParenthesisToken = lexer.SyntaxTokens.First();

        Assert.Equal(SyntaxKind.CloseParenthesisToken, closeParenthesisToken.SyntaxKind);

        var text = closeParenthesisToken.BlazorStudioTextSpan.GetText(sourceText);
        Assert.Equal(closeParenthesisTokenAsString, text);
    }
	
	[Fact]
    public void SHOULD_LEX_OPEN_BRACE_TOKEN()
    {
        var openBraceTokenAsString = "{";
        var sourceText = $"{openBraceTokenAsString}".ReplaceLineEndings("\n");

        var lexer = new Lexer(sourceText);
        lexer.Lex();

        var openBraceToken = lexer.SyntaxTokens.First();

        Assert.Equal(SyntaxKind.OpenBraceToken, openBraceToken.SyntaxKind);

        var text = openBraceToken.BlazorStudioTextSpan.GetText(sourceText);
        Assert.Equal(openBraceTokenAsString, text);
    }
	
	[Fact]
    public void SHOULD_LEX_CLOSE_BRACE_TOKEN()
    {
        var closeBraceTokenAsString = "}";
        var sourceText = $"{closeBraceTokenAsString}".ReplaceLineEndings("\n");

        var lexer = new Lexer(sourceText);
        lexer.Lex();

        var closeBraceToken = lexer.SyntaxTokens.First();

        Assert.Equal(SyntaxKind.CloseBraceToken, closeBraceToken.SyntaxKind);

        var text = closeBraceToken.BlazorStudioTextSpan.GetText(sourceText);
        Assert.Equal(closeBraceTokenAsString, text);
    }
}