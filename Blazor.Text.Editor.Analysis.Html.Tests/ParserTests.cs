using Blazor.Text.Editor.Analysis.Html.ClassLib;
using Blazor.Text.Editor.Analysis.Html.ClassLib.SyntaxActors;
using Blazor.Text.Editor.Analysis.Html.ClassLib.SyntaxItems;

namespace Blazor.Text.Editor.Analysis.Html.Tests;

public class ParserTests
{
    [Fact]
    public Task TagTextSyntax()
    {
        var content = @"some text";

        var htmlSyntaxUnit = HtmlSyntaxTree.ParseText(content);

        var syntaxNodeRoot = htmlSyntaxUnit.RootTagSyntax;

        Assert.Single(syntaxNodeRoot.ChildTagSyntaxes);

        var childTagSyntax = syntaxNodeRoot.ChildTagSyntaxes.Single();

        Assert.IsType<TagTextSyntax>(childTagSyntax);

        Assert.Equal(
            content,
            ((TagTextSyntax)childTagSyntax).Value);
        return Task.CompletedTask;
    }

    // TODO: Lacks Asserts
    // [Fact]
    // public Task TagWithChildContentOfText()
    // {
    //     var textNodeContent = "Apple Sauce";
    //
    //     var content = $@"<div>{textNodeContent}</div>";
    //
    //     var htmlSyntaxUnit = HtmlSyntaxTree.ParseText(content);
    //
    //     var syntaxNodeRoot = htmlSyntaxUnit.RootTagSyntax;
    //     return Task.CompletedTask;
    // }

    // TODO: Lacks Asserts
    // [Fact]
    // public Task ErroneousInfiniteLoopInThisTestCase()
    // {
    //     var content = File.ReadAllText(
    //         @"C:\Users\hunte\source\BlazorCrudApp\BlazorCrudApp.WebAssembly\Client\Shared\MainLayout.razor");
    //
    //     var htmlSyntaxUnit = HtmlSyntaxTree.ParseText(content);
    //
    //     var syntaxNodeRoot = htmlSyntaxUnit.RootTagSyntax;
    //     return Task.CompletedTask;
    // }

    // TODO: Lacks Asserts
//     [Fact]
//     public async Task Test3()
//     {
//         var content = @"some text
// <div>Apple Sauce</div>";
//
//         /*
//          * Expected:
//          *     -TextNode = 'some text'
//          *     -TagDiv
//          *         -TextNode = 'Apple Sauce' 
//          */
//
//         var lexer = new TextEditorHtmlLexer();
//
//         var textEditorTextSpans =
//             await lexer.Lex(content);
//     }

    // TODO: Lacks Asserts
    // [Fact]
    // public async Task Test4()
    // {
    //     var content =
    //         File.ReadAllText(
    //             @"C:\Users\hunte\source\BlazorCrudApp\BlazorCrudApp.WebAssembly\Client\Shared\MainLayout.razor");
    //
    //     /*
    //      * Expected:
    //      *     -TextNode = 'some text'
    //      *     -TagDiv
    //      *         -TextNode = 'Apple Sauce' 
    //      */
    //
    //     var lexer = new TextEditorHtmlLexer();
    //
    //     var textEditorTextSpans =
    //         await lexer.Lex(content);
    // }
}