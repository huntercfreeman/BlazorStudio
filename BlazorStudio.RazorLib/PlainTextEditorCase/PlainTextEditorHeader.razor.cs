using BlazorStudio.ClassLib.Store.PlainTextEditorCase;
using BlazorStudio.ClassLib.Store.SolutionCase;
using Fluxor;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.PlainTextEditorCase;

public partial class PlainTextEditorHeader : ComponentBase
{
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;
    [Inject]
    private IState<SolutionState> SolutionState { get; set; } = null!;

    [Parameter]
    public IPlainTextEditor PlainTextEditor { get; set; } = null!;
    [Parameter]
    public PlainTextEditorDisplay.WidthAndHeightTestResult? WidthAndHeightTestResult { get; set; }
    
    private int _fontSize;

    private int FontSize
    {
        get => _fontSize;
        set
        {
            _fontSize = value;
            Dispatcher.Dispatch(new PlainTextEditorSetFontSizeAction(PlainTextEditor.PlainTextEditorKey, _fontSize));
        }
    }

    protected override void OnParametersSet()
    {
        _fontSize = PlainTextEditor.RichTextEditorOptions.FontSizeInPixels;

        base.OnParametersSet();
    }
}