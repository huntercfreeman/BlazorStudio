using Blazor.Text.Editor.Analysis.CSharp.ClassLib;
using BlazorTextEditor.RazorLib.TextEditor;

namespace Blazor.Text.Editor.Analysis.CSharp.Tests;

/// <summary>
/// The <see cref="IndividualDecorationTests"/> are to provide as input to the test
/// a string containing the bare minimum necessary for an ILexer to correctly
/// identify a token. This string does not need to be compilable.
/// <br/><br/>
/// The test itself is to assert that the ILexer is identifying that
/// token, using the bare minimum input, correctly.
/// </summary>
public class IndividualDecorationTests
{
    [Fact]
    public async Task DecorateNone()
    {
        var input = "x";
        var lexer = new TextEditorCSharpLexer();
        var decorationMapper = new TextEditorCSharpDecorationMapper();

        var textEditor = new TextEditorBase(
            input,
            lexer,
            decorationMapper);

        await textEditor.ApplySyntaxHighlightingAsync();

        var richCharacters = textEditor.GetAllRichCharacters();
        
        Assert.Equal(
            CSharpDecorationKind.None, 
            (CSharpDecorationKind)richCharacters.First().DecorationByte);
    }
    
    [Fact]
    public async Task DecorateMethod()
    {
        var nameOfMethod = "MyMethod";
        
        var input = $@"public class MyClass
{{
    public void {nameOfMethod}() 
    {{
    }}
}}";
        
        var lexer = new TextEditorCSharpLexer();
        var decorationMapper = new TextEditorCSharpDecorationMapper();

        var textEditor = new TextEditorBase(
            input,
            lexer,
            decorationMapper);

        await textEditor.ApplySyntaxHighlightingAsync();

        var richCharacters = textEditor.GetAllRichCharacters();

        var methodIdentifierRichCharacters = richCharacters
            .Skip(41)
            .Take(nameOfMethod.Length)
            .ToArray();

        Assert.True(methodIdentifierRichCharacters
            .All(x => 
                ((CSharpDecorationKind)x.DecorationByte) == CSharpDecorationKind.Method));

        var stringifiedMethodIdentifier = new string(methodIdentifierRichCharacters
            .Select(x => x.Value)
            .ToArray());
        
        Assert.Equal(
            nameOfMethod, 
            stringifiedMethodIdentifier);
    }
    
    [Fact]
    public async Task DecorateType()
    {
        var nameOfType = "MyClass";
        
        var input = $@"public class {nameOfType}
{{
}}";
        
        var lexer = new TextEditorCSharpLexer();
        var decorationMapper = new TextEditorCSharpDecorationMapper();

        var textEditor = new TextEditorBase(
            input,
            lexer,
            decorationMapper);

        await textEditor.ApplySyntaxHighlightingAsync();

        var richCharacters = textEditor.GetAllRichCharacters();

        var typeIdentifierRichCharacters = richCharacters
            .Skip(13)
            .Take(nameOfType.Length)
            .ToArray();

        Assert.True(typeIdentifierRichCharacters
            .All(x => 
                ((CSharpDecorationKind)x.DecorationByte) == CSharpDecorationKind.Type));

        var stringifiedTypeIdentifier = new string(typeIdentifierRichCharacters
            .Select(x => x.Value)
            .ToArray());
        
        Assert.Equal(
            nameOfType, 
            stringifiedTypeIdentifier);
    }
    
    [Fact]
    public async Task DecorateParameter()
    {
        var nameOfParameter = "myParameter";
        
        var input = $@"public class MyClass
{{
    public void MyMethod(int {nameOfParameter}) 
    {{
    }}
}}";
        
        var lexer = new TextEditorCSharpLexer();
        var decorationMapper = new TextEditorCSharpDecorationMapper();

        var textEditor = new TextEditorBase(
            input,
            lexer,
            decorationMapper);

        await textEditor.ApplySyntaxHighlightingAsync();

        var richCharacters = textEditor.GetAllRichCharacters();

        var parameterIdentifierRichCharacters = richCharacters
            .Skip(54)
            .Take(nameOfParameter.Length)
            .ToArray();

        Assert.True(parameterIdentifierRichCharacters
            .All(x => 
                ((CSharpDecorationKind)x.DecorationByte) == CSharpDecorationKind.Parameter));

        var stringifiedParameter = new string(parameterIdentifierRichCharacters
            .Select(x => x.Value)
            .ToArray());
        
        Assert.Equal(
            nameOfParameter, 
            stringifiedParameter);
    }
    
    [Fact]
    public async Task DecorateStringLiteral()
    {
        var stringLiteral = "\"Hello World!\"";
        
        var input = $@"public class MyClass
{{
    public string MyMethod() 
    {{
        return {stringLiteral};
    }}
}}";
        
        var lexer = new TextEditorCSharpLexer();
        var decorationMapper = new TextEditorCSharpDecorationMapper();

        var textEditor = new TextEditorBase(
            input,
            lexer,
            decorationMapper);

        await textEditor.ApplySyntaxHighlightingAsync();

        var richCharacters = textEditor.GetAllRichCharacters();

        var stringLiteralRichCharacters = richCharacters
            .Skip(78)
            .Take(stringLiteral.Length)
            .ToArray();

        Assert.True(stringLiteralRichCharacters
            .All(x => 
                ((CSharpDecorationKind)x.DecorationByte) == CSharpDecorationKind.StringLiteral));

        var stringifiedStringLiteral = new string(stringLiteralRichCharacters
            .Select(x => x.Value)
            .ToArray());
        
        Assert.Equal(
            stringLiteral, 
            stringifiedStringLiteral);
    }
    
    [Fact]
    public async Task DecorateKeyword()
    {
        var keyword = "public";
        
        var input = $@"{keyword} class MyClass
{{
    public void MyMethod() 
    {{
    }}
}}";
        
        var lexer = new TextEditorCSharpLexer();
        var decorationMapper = new TextEditorCSharpDecorationMapper();

        var textEditor = new TextEditorBase(
            input,
            lexer,
            decorationMapper);

        await textEditor.ApplySyntaxHighlightingAsync();

        var richCharacters = textEditor.GetAllRichCharacters();

        var keywordRichCharacters = richCharacters
            .Skip(0)
            .Take(keyword.Length)
            .ToArray();

        Assert.True(keywordRichCharacters
            .All(x => 
                ((CSharpDecorationKind)x.DecorationByte) == CSharpDecorationKind.Keyword));

        var stringifiedKeyword = new string(keywordRichCharacters
            .Select(x => x.Value)
            .ToArray());
        
        Assert.Equal(
            keyword, 
            stringifiedKeyword);
    }
    
    [Fact]
    public async Task DecorateSingleLineComment()
    {
        var comment = "// This is a comment";
        
        var input = $@"{comment}
public class MyClass
{{
    public void MyMethod() 
    {{
    }}
}}";
        
        var lexer = new TextEditorCSharpLexer();
        var decorationMapper = new TextEditorCSharpDecorationMapper();

        var textEditor = new TextEditorBase(
            input,
            lexer,
            decorationMapper);

        await textEditor.ApplySyntaxHighlightingAsync();

        var richCharacters = textEditor.GetAllRichCharacters();

        var commentRichCharacters = richCharacters
            .Skip(0)
            .Take(comment.Length)
            .ToArray();

        Assert.True(commentRichCharacters
            .All(x => 
                ((CSharpDecorationKind)x.DecorationByte) == CSharpDecorationKind.Comment));

        var stringifiedComment = new string(commentRichCharacters
            .Select(x => x.Value)
            .ToArray());
        
        Assert.Equal(
            comment, 
            stringifiedComment);
    }
    
    [Fact]
    public async Task DecorateMultiLineComment()
    {
        var comment = @"/* 
This 
is a
comment 
*/";
        
        var input = $@"{comment}
public class MyClass
{{
    public void MyMethod() 
    {{
    }}
}}";
        
        var lexer = new TextEditorCSharpLexer();
        var decorationMapper = new TextEditorCSharpDecorationMapper();

        var textEditor = new TextEditorBase(
            input,
            lexer,
            decorationMapper);

        await textEditor.ApplySyntaxHighlightingAsync();

        var richCharacters = textEditor.GetAllRichCharacters();

        var commentRichCharacters = richCharacters
            .Skip(0)
            .Take(comment.Length)
            .ToArray();

        Assert.True(commentRichCharacters
            .All(x => 
                ((CSharpDecorationKind)x.DecorationByte) == CSharpDecorationKind.Comment));

        var stringifiedComment = new string(commentRichCharacters
            .Select(x => x.Value)
            .ToArray());
        
        Assert.Equal(
            comment, 
            stringifiedComment);
    }
}