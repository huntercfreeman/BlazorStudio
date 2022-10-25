using Blazor.Text.Editor.Analysis.Html.ClassLib;
using Blazor.Text.Editor.Analysis.Html.ClassLib.SyntaxActors;
using Blazor.Text.Editor.Analysis.Razor.ClassLib;

namespace Blazor.Text.Editor.Analysis.Razor.Tests;

public class ParserTests
{
    [Fact]
    public async Task TagTextSyntax()
    {
        var content = @"<div class=""bstudio_counter"">
	<button class=""btn btn-primary""
	        @onclick=""IncrementCount"">
	        
	        Increment
	</button>
	
	&nbsp;
	
	(@_count)
</div>

@code{
	private int _count = 0;
	
	private void IncrementCount()
	{
		_count++;
	}
}";

        var htmlSyntaxUnit = HtmlSyntaxTree.ParseText(
	        content,
	        RazorInjectedLanguageFacts.RazorInjectedLanguageDefinition);
        
        var syntaxNodeRoot = htmlSyntaxUnit.RootTagSyntax;
    }
}