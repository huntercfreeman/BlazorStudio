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
    [CascadingParameter(Name="NotifyCharacterWasClicked")]
    public Action NotifyCharacterWasClicked { get; set; } = null!;
    [CascadingParameter]
    public RichTextEditorOptions RichTextEditorOptions { get; set; } = null!;
    [CascadingParameter(Name="GetWidthAndHeightTest")]
    public bool GetWidthAndHeightTest { get; set; }
    [CascadingParameter(Name="SelectionSpan")] 
    public SelectionSpanRecord? SelectionSpan { get; set; }
    [CascadingParameter(Name="StartOfSpan")] 
    public long StartOfSpan { get; set; }
    [CascadingParameter(Name="IsSubscribedToDragState")] 
    public bool IsSubscribedToDragState { get; set; }
    [CascadingParameter(Name = "PlainTextEditorOnClickActionSemaphoreSlim")]
    public SemaphoreSlim PlainTextEditorOnClickActionSemaphoreSlim { get; set; } = null!;

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
        NotifyCharacterWasClicked();

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
        if (!IsSubscribedToDragState)
        {
            return;
        }
        
        var success = await PlainTextEditorOnClickActionSemaphoreSlim
            .WaitAsync(TimeSpan.Zero);

        if (!success)
            return;
        
        try
        {
            NotifyCharacterWasClicked();
        
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
            PlainTextEditorOnClickActionSemaphoreSlim.Release();
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
