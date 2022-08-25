using BlazorStudio.ClassLib.Store.PlainTextEditorCase;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.PlainTextEditorCase;

public partial class PlainTextEditorCursorDisplay : ComponentBase
{
    [CascadingParameter(Name="CurrentRowIndex")]
    public int CurrentRowIndex { get; set; }
    [CascadingParameter(Name="CurrentCharacterColumnIndex")]
    public int CurrentCharacterColumnIndex { get; set; }
    [CascadingParameter]
    public RichTextEditorOptions RichTextEditorOptions { get; set; } = null!;

    [Parameter, EditorRequired]
    public int MostDigitsInARowNumber { get; set; }

    private int _lineNumberMarginRight = 1;
    private ElementReference? _textAreaElementReference;
    
    public string CursorElementId { get; } = "pte_" + Guid.NewGuid();
    
    private int LineNumberOffset => MostDigitsInARowNumber + _lineNumberMarginRight;
    
    private string GetPositionCssStyling => 
        $"top: {CurrentRowIndex * RichTextEditorOptions.HeightOfARowInPixels}px;" +
        $"left: {(CurrentCharacterColumnIndex + LineNumberOffset) * RichTextEditorOptions.WidthOfACharacterInPixels}px;";

    private string GetWidthHeightCssStyling =>
        $"width: 1.5px;" +
        $"height: {RichTextEditorOptions.HeightOfARowInPixels}px;";

    public async Task FocusAsync()
    {
        if (_textAreaElementReference is not null)
        {
            await _textAreaElementReference.Value.FocusAsync();
        }
    }
}