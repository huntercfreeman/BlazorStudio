using BlazorStudio.ClassLib.Sequence;
using BlazorStudio.ClassLib.Store.PlainTextEditorCase;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;

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

    [Parameter, EditorRequired]
    public IPlainTextEditorRow PlainTextEditorRow { get; set; } = null!;
    [Parameter, EditorRequired]
    public int MostDigitsInARowNumber { get; set; }
    [Parameter]
    public bool GetWidthAndHeightTest { get; set; }
    [Parameter]
    public string WidthAndHeightTestId { get; set; }

    private bool _characterWasClicked;
    private SequenceKey? _previousSequenceKey;
    private int _previousMostDigitsInARowNumber;
    private bool _previousShouldDisplay;

    private string IsActiveCss => PlainTextEditorCurrentRowIndex == RowIndex
        ? "pte_active"
        : string.Empty;

    private string WidthStyleCss => $"width: calc(100% - {MostDigitsInARowNumber}ch);";

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

    private void DispatchPlainTextEditorOnClickAction()
    {
        if (!_characterWasClicked)
        {
            Dispatcher.Dispatch(
                new PlainTextEditorOnClickAction(
                    PlainTextEditorKey,
                    RowIndex,
                    PlainTextEditorRow.Tokens.Count - 1,
                    null,
                    CancellationToken.None
                )
            );
        }
        else
        {
            _characterWasClicked = false;
        }
    }
}
