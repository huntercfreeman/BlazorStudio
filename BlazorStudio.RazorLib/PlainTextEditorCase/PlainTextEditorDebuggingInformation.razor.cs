using BlazorStudio.ClassLib.Store.PlainTextEditorCase;
using BlazorStudio.ClassLib.Store.SolutionCase;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.CodeAnalysis;
using static BlazorStudio.RazorLib.PlainTextEditorCase.PlainTextEditorDisplay;

namespace BlazorStudio.RazorLib.PlainTextEditorCase;

public partial class PlainTextEditorDebuggingInformation : FluxorComponent
{
    [Inject] 
    private IState<SolutionState> SolutionStateWrap { get; set; } = null!;
    
    [Parameter]
    public IPlainTextEditor PlainTextEditor { get; set; } = null!;
    [Parameter]
    public WidthAndHeightTestResult? WidthAndHeightTestResult { get; set; }

    private List<(SyntaxNode syntaxNode, string? fileName)> _documentSyntaxNodeTuples =
        new List<(SyntaxNode syntaxNode, string? fileName)>();
    
    private List<(SyntaxNode syntaxNode, string? fileName)> _additionalDocumentSyntaxNodeTuples =
        new List<(SyntaxNode syntaxNode, string? fileName)>();
    
    private async Task GetSyntaxRootsOnClick()
    {
        var currentSolutionState = SolutionStateWrap.Value;

        _documentSyntaxNodeTuples.Clear();

        foreach (var documentTuple in currentSolutionState.FileAbsoluteFilePathToDocumentMap)
        {
            var root = await documentTuple.Value.Document.GetSyntaxRootAsync();
            
            _documentSyntaxNodeTuples.Add((root, documentTuple.Key.AbsoluteFilePathString));
        }
        
        _additionalDocumentSyntaxNodeTuples.Clear();

        // foreach (var additionalDocumentTuple in currentSolutionState.FileAbsoluteFilePathToAdditionalDocumentMap)
        // {
        //     var root = await additionalDocumentTuple.Value.TextDocument.R.GetSyntaxRootAsync();
        //     
        //     _additionalDocumentSyntaxNodeTuples.Add((root, additionalDocumentTuple.Key.AbsoluteFilePathString));
        // }
    }

    private MarkupString NullSafeToMarkupString(string name, object? obj)
    {
        return (MarkupString)(name + "&nbsp;->&nbsp;" + obj?.ToString() ?? "null");
    }
}