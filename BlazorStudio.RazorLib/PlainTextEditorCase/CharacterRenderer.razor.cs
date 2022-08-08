using BlazorStudio.ClassLib.Store.PlainTextEditorCase;
using Fluxor;
using Microsoft.AspNetCore.Components;

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

    private string WidthStyleString => GetWidthAndHeightTest
        ? string.Empty
        : $"width: {RichTextEditorOptions.WidthOfACharacterInPixels}px;";

    private void DispatchPlainTextEditorOnClickAction()
    {
        NotifyCharacterWasClicked();

        Dispatcher.Dispatch(
            new PlainTextEditorOnClickAction(
                PlainTextEditorKey,
                RowIndex,
                TokenIndex,
                CharacterIndex,
                CancellationToken.None
            )
        );
    }
}
