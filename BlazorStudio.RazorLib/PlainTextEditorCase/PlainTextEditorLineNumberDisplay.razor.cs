using BlazorStudio.ClassLib.Store.PlainTextEditorCase;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.PlainTextEditorCase;

public partial class PlainTextEditorLineNumberDisplay : ComponentBase
{
    [CascadingParameter(Name="RowIndex")]
    public int RowIndex { get; set; }
    [CascadingParameter(Name = "RowIndexOffset")]
    public int RowIndexOffset { get; set; }
    [CascadingParameter]
    public RichTextEditorOptions RichTextEditorOptions { get; set; } = null!;
    [CascadingParameter(Name = "GetWidthAndHeightTest")]
    public bool GetWidthAndHeightTest { get; set; }

    [Parameter, EditorRequired]
    public int MostDigitsInARowNumber { get; set; }

    private int CountOfDigitsInRowNumber => (RowIndex + RowIndexOffset + 1).ToString().Length;

    private string WidthStyleCss => GetWidthAndHeightTest
        ? $"width: {MostDigitsInARowNumber}ch;"
        : $"width: {RichTextEditorOptions.WidthOfACharacterInPixels}px;";

    private string PaddingLeftStyleCss => GetWidthAndHeightTest
        ? $"padding-left: {MostDigitsInARowNumber - CountOfDigitsInRowNumber}ch;"
        : $"padding-left: {(MostDigitsInARowNumber - CountOfDigitsInRowNumber) * RichTextEditorOptions.WidthOfACharacterInPixels}px;";
    
    private string MarginRightStyleCss => GetWidthAndHeightTest
        ? $"margin-right: 1ch;"
        : $"margin-right: {RichTextEditorOptions.WidthOfACharacterInPixels}px;";
}
