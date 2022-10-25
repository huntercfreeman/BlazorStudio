using Blazor.Text.Editor.Analysis.CSharp.ClassLib;
using BlazorTextEditor.RazorLib.TextEditor;

namespace Blazor.Text.Editor.Analysis.CSharp.Tests;

/// <summary>
/// The <see cref="SourceCodeDecorationTests"/> are to provide as input
/// to the test a string containing the full source code of a C# snippet.
/// This snippet does not need to include invocations to other methods.
/// <br/><br/>
/// For example the method:
/// 'private void MyMethod() { Console.WriteLine("Hello World!"); }'
/// <br/><br/>
/// may be used as input as it is presumed to be a snippet of a compilable program.
/// <br/><br/>
/// For more specific tests, such as
/// the input: "Hello World!", one should refer
/// to <see cref="IndividualDecorationTests"/>
/// </summary>
public class SourceCodeDecorationTests
{
    [Fact]
    public async Task DecorateHelloWorld()
    {
        // https://www.programiz.com/csharp-programming/hello-world

        var testTextSpans = new List<TestTextSpan>
        {
            new TestTextSpan(
                "// Hello World! program",
                0,
                CSharpDecorationKind.Comment),
            new TestTextSpan(
                "namespace",
                25,
                CSharpDecorationKind.Keyword),
            new TestTextSpan(
                "HelloWorld",
                35,
                CSharpDecorationKind.None),
            new TestTextSpan(
                "class",
                54,
                CSharpDecorationKind.Keyword),
            new TestTextSpan(
                "Hello",
                60,
                CSharpDecorationKind.Type),
            new TestTextSpan(
                "static",
                86,
                CSharpDecorationKind.Keyword),
            new TestTextSpan(
                "void",
                93,
                CSharpDecorationKind.Keyword),
            new TestTextSpan(
                "Main",
                98,
                CSharpDecorationKind.Method),
            new TestTextSpan(
                "string",
                103,
                CSharpDecorationKind.Keyword),
            new TestTextSpan(
                "args",
                112,
                CSharpDecorationKind.Parameter),
            new TestTextSpan(
                "System.Console",
                142,
                CSharpDecorationKind.None),
            new TestTextSpan(
                "WriteLine",
                157,
                CSharpDecorationKind.Method),
            new TestTextSpan(
                "\"Hello World!\"",
                167,
                CSharpDecorationKind.StringLiteral)
        };

        var input = @"// Hello World! program
namespace HelloWorld
{
    class Hello {         
        static void Main(string[] args)
        {
            System.Console.WriteLine(""Hello World!"");
        }
    }
}";

        var lexer = new TextEditorCSharpLexer();
        var decorationMapper = new TextEditorCSharpDecorationMapper();

        var textEditor = new TextEditorBase(
            input,
            lexer,
            decorationMapper,
            null);

        await textEditor.ApplySyntaxHighlightingAsync();

        var richCharacters = textEditor.GetAllRichCharacters();

        foreach (var testTextSpan in testTextSpans)
        {
            var textTextSpanRichCharacters = richCharacters
                .Skip(testTextSpan.StartingIndex)
                .Take(testTextSpan.Content.Length)
                .ToArray();

            Assert.True(textTextSpanRichCharacters
                .All(x =>
                    ((CSharpDecorationKind)x.DecorationByte) == testTextSpan.CSharpDecorationKind));

            var stringifiedTestTextSpan = new string(textTextSpanRichCharacters
                .Select(x => x.Value)
                .ToArray());

            Assert.Equal(
                testTextSpan.Content,
                stringifiedTestTextSpan);
        }
    }

    private record TestTextSpan(
        string Content,
        int StartingIndex,
        CSharpDecorationKind CSharpDecorationKind);
}