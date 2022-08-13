using BlazorStudio.ClassLib.Store.PlainTextEditorCase;
using Fluxor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorStudio.RazorLib.PlainTextEditorCase;

public partial class CharacterRenderer : ComponentBase
{
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;
    
    [CascadingParameter]
    public bool IsFocused { get; set; }
    [CascadingParameter(Name="TokenIndex")]
    public int TokenIndex { get; set; }
    [CascadingParameter(Name="RowIndex")]
    public int RowIndex { get; set; }
    [CascadingParameter]
    public PlainTextEditorKey PlainTextEditorKey { get; set; } = null!;
    [CascadingParameter]
    public RichTextEditorOptions RichTextEditorOptions { get; set; } = null!;
    [CascadingParameter(Name="GetWidthAndHeightTest")]
    public bool GetWidthAndHeightTest { get; set; }
    [CascadingParameter(Name="SelectionSpan")] 
    public SelectionSpanRecord? SelectionSpan { get; set; }
    [CascadingParameter(Name="StartOfSpan")] 
    public long StartOfSpan { get; set; }
    [CascadingParameter(Name="IsMouseSelectingText")] 
    public bool IsMouseSelectingText { get; set; }
    [CascadingParameter(Name="MouseTextSelectionSemaphoreSlim")]
    public SemaphoreSlim MouseTextSelectionSemaphoreSlim { get; set; } = null!;
    [CascadingParameter(Name="FocusPlainTextEditor")]
    public Action FocusPlainTextEditor { get; set; } = null!;

    /// <summary>
    /// The html escaped character for space is nbsp which
    /// requires more than 1 character to represent therefore this is of type string
    /// </summary>
    [Parameter, EditorRequired]
    public string Character { get; set; } = null!;
    [Parameter, EditorRequired]
    public int CharacterIndex { get; set; }
    [Parameter, EditorRequired]
    public bool ShouldDisplayCursor { get; set; }

    private string _selectedCssClassString = "pte_plain-text-editor-character-selected";

    private string WidthStyleString => GetWidthAndHeightTest
        ? string.Empty
        : $"width: {RichTextEditorOptions.WidthOfACharacterInPixels}px;";

    private string IsSelectedCssClassString => GetIsSelectedCssClassString();
    
    private long CharacterColumnIndex => StartOfSpan + CharacterIndex;
    
    private void DispatchPlainTextEditorOnClickAction(MouseEventArgs mouseEventArgs)
    {
        // Clicking results in <input /> losing focus so re-focus
        FocusPlainTextEditor();
        
        Dispatcher.Dispatch(
            new PlainTextEditorOnClickAction(
                PlainTextEditorKey,
                RowIndex,
                TokenIndex,
                CharacterIndex,
                mouseEventArgs.ShiftKey,
                CancellationToken.None
            )
        );
    }
    
    private async Task SelectTextOnMouseOverAsync(MouseEventArgs mouseEventArgs)
    {
        if (!IsMouseSelectingText)
        {
            return;
        }
        
        var success = await MouseTextSelectionSemaphoreSlim
            .WaitAsync(TimeSpan.Zero);

        if (!success)
            return;
        
        try
        {
            Dispatcher.Dispatch(
                new PlainTextEditorOnMouseOverCharacterAction(
                    PlainTextEditorKey,
                    RowIndex,
                    TokenIndex,
                    CharacterIndex,
                    true,
                    CancellationToken.None
                )
            );
        }
        finally
        {
            MouseTextSelectionSemaphoreSlim.Release();
        }
    }
    
    private string GetIsSelectedCssClassString()
    {
        if (SelectionSpan is null)
            return string.Empty;

        if (SelectionSpan.SelectionDirection == SelectionDirection.Left)
        {
            if (CharacterColumnIndex <= SelectionSpan.InclusiveStartingDocumentTextIndex &&
                CharacterColumnIndex > SelectionSpan.ExclusiveEndingDocumentTextIndex)
            {
                return _selectedCssClassString;
            }
        }
        else
        {
            if (CharacterColumnIndex >= SelectionSpan.InclusiveStartingDocumentTextIndex &&
                CharacterColumnIndex < SelectionSpan.ExclusiveEndingDocumentTextIndex)
            {
                return _selectedCssClassString;
            }
        }
        
        return string.Empty;
    }
}
