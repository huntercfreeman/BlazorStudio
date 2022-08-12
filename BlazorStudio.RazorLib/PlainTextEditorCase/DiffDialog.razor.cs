using BlazorStudio.ClassLib.Store.EditorCase;
using BlazorStudio.ClassLib.Store.PlainTextEditorCase;
using BlazorStudio.ClassLib.Store.SolutionCase;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;
using static BlazorStudio.RazorLib.PlainTextEditorCase.PlainTextEditorDisplay;

namespace BlazorStudio.RazorLib.PlainTextEditorCase;

public partial class DiffDialog : FluxorComponent
{
    [Inject]
    private IStateSelection<PlainTextEditorStates, IPlainTextEditor?> PlainTextEditorSelector { get; set; } = null!;
    [Inject]
    private IState<SolutionState> SolutionStateWrap { get; set; } = null!;
    
    [Parameter, EditorRequired]
    public PlainTextEditorKey PlainTextEditorKey { get; set; } = null!;
    [Parameter]
    public WidthAndHeightTestResult? WidthAndHeightTestResult { get; set; }

    private string _plainText = string.Empty;

    protected override void OnInitialized()
    {
        PlainTextEditorSelector.Select(x =>
        {
            x.Map.TryGetValue(PlainTextEditorKey, out var value);
            return value;
        });

        base.OnInitialized();
    }

    private void GetPlainText(IPlainTextEditor currentPlainTextEditor)
    {
        _plainText = currentPlainTextEditor.GetDocumentPlainText();
    }

    private async Task SaveChanges(IPlainTextEditor currentPlainTextEditor)
    {
        if (_plainText is null)
        {
            return;
        }

        await currentPlainTextEditor.FileHandle.SaveAsync(_plainText, CancellationToken.None);

        _plainText = "saved text";
    }
}