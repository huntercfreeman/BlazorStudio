using BlazorStudio.ClassLib.Sequence;
using BlazorStudio.ClassLib.Store.TextEditorCase;
using BlazorStudio.ClassLib.TextEditor;
using BlazorStudio.ClassLib.TextEditor.IndexWrappers;
using BlazorStudio.RazorLib.ShouldRender;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.TextEditorCase;

public partial class TextEditorDisplay : FluxorComponent
{
    [Inject]
    private IStateSelection<TextEditorStates, TextEditorBase> TextEditorStatesSelection { get; set; } = null!;

    [Parameter, EditorRequired]
    public TextEditorKey TextEditorKey { get; set; } = null!;

    private bool _colorFlip;
    private TextPartition? _textPartition;
    private SequenceKey _previousTextPartitionSequenceKey = SequenceKey.Empty();

    private string BackgroundColor => GetBackgroundColor();

    protected override void OnInitialized()
    {
        TextEditorStatesSelection
            .Select(x => x.TextEditorMap[TextEditorKey]);
        
        base.OnInitialized();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            // I don't want the IMMUTABLE state changing due to Blazor using a MUTABLE reference.
            var localTextEditorStates = TextEditorStatesSelection.Value;

            _textPartition = localTextEditorStates.GetTextPartition(new RectangularCoordinates(
                TopLeftCorner: (new(0), new(0)),
                BottomRightCorner: (new(Int32.MaxValue), new(10))));

            await InvokeAsync(StateHasChanged);
        }
        
        await base.OnAfterRenderAsync(firstRender);
    }

    private string GetBackgroundColor()
    {
        var localColorFlip = _colorFlip;
        _colorFlip = !_colorFlip;

        return localColorFlip
            ? "var(--bstudio_primary-background-color)"
            : "var(--bstudio_secondary-background-color)";
    }
    
    private bool ShouldRenderFunc(ShouldRenderBoundary.IsFirstShouldRenderValue firstShouldRender)
    {
        var shouldRender = _textPartition is not null &&
               _previousTextPartitionSequenceKey != _textPartition.SequenceKey;

        _previousTextPartitionSequenceKey = _textPartition?.SequenceKey ?? SequenceKey.Empty();

        return shouldRender;
    }
}