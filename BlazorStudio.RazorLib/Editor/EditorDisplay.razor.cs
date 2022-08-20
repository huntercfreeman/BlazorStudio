using BlazorStudio.ClassLib.Contexts;
using BlazorStudio.ClassLib.FileSystem.Classes;
using BlazorStudio.ClassLib.FileSystemApi;
using BlazorStudio.ClassLib.FileSystemApi.MemoryMapped;
using BlazorStudio.ClassLib.Store.ContextCase;
using BlazorStudio.ClassLib.Store.EditorCase;
using BlazorStudio.ClassLib.Store.PlainTextEditorCase;
using BlazorStudio.ClassLib.UserInterface;
using BlazorStudio.RazorLib.ContextCase;
using BlazorStudio.RazorLib.PlainTextEditorCase;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.Editor;

public partial class EditorDisplay : FluxorComponent
{
    [Inject]
    private IState<EditorState> EditorStateWrap { get; set; } = null!;
    [Inject]
    private IState<PlainTextEditorStates> PlainTextEditorStatesWrap { get; set; } = null!;
    [Inject]
    private IFileSystemProvider FileSystemProvider { get; set; } = null!;
    [Inject]
    private IStateSelection<ContextState, ContextRecord> ContextStateSelector { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    [Parameter, EditorRequired]
    public ClassLib.UserInterface.Dimensions Dimensions { get; set; } = null!;
    
    private ContextBoundary _contextBoundary;
    private ElementReference? _editorDisplayElementReference;

    protected override void OnInitialized()
    {
        ContextStateSelector
            .Select(x => x.ContextRecords[ContextFacts.EditorDisplayContext.ContextKey]);
        
        base.OnInitialized();
    }

    protected override Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            ContextStateSelector.Value.OnFocusRequestedEventHandler += ValueOnOnFocusRequestedEventHandler;
        }
        
        return base.OnAfterRenderAsync(firstRender);
    }

    private async void ValueOnOnFocusRequestedEventHandler(object? sender, EventArgs e)
    {
        if (_editorDisplayElementReference is not null)
        {
            await _editorDisplayElementReference.Value.FocusAsync();
        }
    }

    private void SetActiveTabIndexOnClick(int tabIndex)
    {
        Dispatcher.Dispatch(new SetActiveTabIndexAction(tabIndex));
    }
    
    private void DisposePlainTextEditorOnClick(PlainTextEditorKey plainTextEditorKey)
    {
        var plainTextEditorStates = PlainTextEditorStatesWrap.Value;
        var editorState = EditorStateWrap.Value;

        // -1 for the to be removed plainTextEditorKey
        // and -1 again to start index from 0
        if (editorState.TabIndex > plainTextEditorStates.Array.Length - 2)
        {
            // Out of bounds of the upcoming Array length

            var nextTabIndex = plainTextEditorStates.Array.Length - 2;

            nextTabIndex = nextTabIndex < 0
                ? 0
                : nextTabIndex;

            Dispatcher.Dispatch(new SetActiveTabIndexAction(nextTabIndex));
        }
        
        Dispatcher.Dispatch(new DeconstructPlainTextEditorRecordAction(plainTextEditorKey));
    }

    protected override void Dispose(bool disposing)
    {
        ContextStateSelector.Value.OnFocusRequestedEventHandler -= ValueOnOnFocusRequestedEventHandler;
        
        base.Dispose(disposing);
    }
}