using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.PlainTextEditorCase;

public partial class PlainTextEditorLineNumberDisplay : ComponentBase
{
    [CascadingParameter(Name="RowIndex")]
    public int RowIndex { get; set; }
    [CascadingParameter(Name= "RowIndexOffset")]
    public int RowIndexOffset { get; set; }

    [Parameter, EditorRequired]
    public int MostDigitsInARowNumber { get; set; }

    private int CountOfDigitsInRowNumber => (RowIndex + RowIndexOffset + 1).ToString().Length;
    private string WidthStyleCss => $"width: {MostDigitsInARowNumber}ch;";
    private string PaddingLeftStyleCss => $"padding-left: {MostDigitsInARowNumber - CountOfDigitsInRowNumber}ch;";
}
