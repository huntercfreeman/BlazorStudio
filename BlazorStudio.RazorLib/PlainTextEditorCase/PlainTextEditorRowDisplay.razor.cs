using BlazorStudio.ClassLib.Sequence;
using BlazorStudio.ClassLib.Store.PlainTextEditorCase;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorStudio.RazorLib.PlainTextEditorCase;

public partial class PlainTextEditorRowDisplay : FluxorComponent
{
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;
    
    [CascadingParameter(Name="CurrentRowIndex")]
    public int PlainTextEditorCurrentRowIndex { get; set; }
    [CascadingParameter(Name="ActiveRowId")]
    public string ActiveRowId { get; set; } = null!;
    [CascadingParameter(Name="RowIndex")]
    public int RowIndex { get; set; }
    [CascadingParameter]
    public PlainTextEditorKey PlainTextEditorKey { get; set; } = null!;
    [CascadingParameter]
    public RichTextEditorOptions RichTextEditorOptions { get; set; } = null!;
    [CascadingParameter(Name="GetWidthAndHeightTest")]
    public bool GetWidthAndHeightTest { get; set; }
    [CascadingParameter(Name="VirtualCharacterIndexMarkerForStartOfARow")]
    public List<long> VirtualCharacterIndexMarkerForStartOfARow { get; set; }
    [CascadingParameter(Name="IsMouseSelectingText")] 
    public bool IsMouseSelectingText { get; set; }
    [CascadingParameter(Name = "MouseTextSelectionSemaphoreSlim")]
    public SemaphoreSlim MouseTextSelectionSemaphoreSlim { get; set; } = null!;

    [Parameter, EditorRequired]
    public IPlainTextEditorRow PlainTextEditorRow { get; set; } = null!;
    [Parameter, EditorRequired]
    public int MostDigitsInARowNumber { get; set; }
    [Parameter]
    public string WidthAndHeightTestId { get; set; }

    private bool _characterWasClicked;
    private SequenceKey? _previousSequenceKey;
    private int _previousMostDigitsInARowNumber;
    private bool _previousIsMouseSelectingText;
    private bool _previousShouldDisplay;
    private int _childOnMouseOver;
    private int _previousCurrentRowIndex;

    private string IsActiveCss => PlainTextEditorCurrentRowIndex == RowIndex
        ? "pte_active"
        : string.Empty;

    
    private string WidthStyleCss => GetWidthAndHeightTest
        ? $"width: calc(100% - {MostDigitsInARowNumber}ch);"
        : $"width: calc(100% - {RichTextEditorOptions.WidthOfACharacterInPixels}px);";

    private string IsActiveRowId => GetActiveRowId();

    protected override bool ShouldRender()
    {
        var shouldRender = false;

        if (PlainTextEditorRow.SequenceKey != _previousSequenceKey ||
            MostDigitsInARowNumber != _previousMostDigitsInARowNumber)
        {
            _previousMostDigitsInARowNumber = MostDigitsInARowNumber;
            shouldRender = true;
        }

        if (IsMouseSelectingText != _previousIsMouseSelectingText)
        {
            _previousIsMouseSelectingText = IsMouseSelectingText;

            if (_previousIsMouseSelectingText)
            {
                // If started selecting text then should render
                // as the selection of text is dependent on
                // calculating character indices for the entire document
                // and this calculation is only done when rendered.
                shouldRender = true;
            }
        }

        _previousSequenceKey = PlainTextEditorRow.SequenceKey;

        return shouldRender;
    }

    private string GetActiveRowId()
    {
        if (!GetWidthAndHeightTest)
        {
            return PlainTextEditorCurrentRowIndex == RowIndex
                ? ActiveRowId
                : string.Empty;
        }
        else
        {
            return WidthAndHeightTestId;
        }
    }

    private void DispatchPlainTextEditorOnClickAction(MouseEventArgs mouseEventArgs)
    {
        Dispatcher.Dispatch(
            new PlainTextEditorOnClickAction(
                PlainTextEditorKey,
                RowIndex,
                PlainTextEditorRow.Tokens.Count - 1,
                null,
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
                    PlainTextEditorRow.Tokens.Count - 1,
                    null,
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
}
