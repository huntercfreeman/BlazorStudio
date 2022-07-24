using BlazorStudio.ClassLib.Store.EditorCase;
using BlazorStudio.ClassLib.Store.PlainTextEditorCase;
using BlazorStudio.ClassLib.UserInterface;
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
    private IDispatcher Dispatcher { get; set; } = null!;

    [Parameter, EditorRequired]
    public ClassLib.UserInterface.Dimensions Dimensions { get; set; } = null!;

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
}