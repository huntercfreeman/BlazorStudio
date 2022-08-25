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

    private string GetPositionCssStyling => 
        $"top: {CurrentRowIndex * RichTextEditorOptions.HeightOfARowInPixels}px;" +
        $"left: {CurrentCharacterColumnIndex * RichTextEditorOptions.WidthOfACharacterInPixels}px;";
    
    private string GetDebuggingCssStyling => $"background-color: orange;";
}