using BlazorStudio.ClassLib.Keyboard;
using BlazorStudio.ClassLib.Store.EditorCase;
using BlazorTextEditor.RazorLib.TextEditor;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorStudio.RazorLib.Editor;

public partial class EditorTabDisplay : FluxorComponent
{
    [Inject]
    private IState<EditorState> EditorStateWrap { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;
    
    [Parameter, EditorRequired]
    public TextEditorBase TextEditor { get; set; } = null!;
    [Parameter, EditorRequired]
    public int TabIndex { get; set; }
    
    // TODO: OLD
    // private string IsActiveCssClass => EditorStateWrap.Value.TextEditorKey == TextEditor.TextEditorKey
    //     ? "bstudio_active"
    //     : string.Empty;

    private void DispatchSetActiveTextEditorKeyAction()
    {
        // TODO: OLD
        // Dispatcher.Dispatch(new SetActiveTextEditorKeyAction(TextEditor.TextEditorKey));
    }
    
    private void FireDisposePlainTextEditorOnClick()
    {
        // TODO: OLD
        // var localTextEditorStates = TextEditorStatesWrap.Value;
        //
        // var activeIndex = localTextEditorStates.TextEditors.IndexOf(TextEditor);
        //
        // var indexOfNextActiveTextEditor = activeIndex;
        //
        // // If last item in list
        // if (activeIndex >= localTextEditorStates.TextEditors.Count - 1)
        // {
        //     indexOfNextActiveTextEditor--;
        // }
        // else
        // {
        //     // ++ operation because nothing yet has been removed.
        //     // The new active TextEditor is set prior to actually removing the current active TextEditor.
        //     indexOfNextActiveTextEditor++;
        // }
        //
        //
        // // List will be empty after disposing
        // if (localTextEditorStates.TextEditors.Count - 1 == 0)
        // {
        //     Dispatcher.Dispatch(new SetActiveTextEditorKeyAction(TextEditorKey.Empty()));
        // }
        // else
        // {
        //     var nextActiveTextEditor = localTextEditorStates.TextEditors[indexOfNextActiveTextEditor];
        //     
        //     Dispatcher.Dispatch(new SetActiveTextEditorKeyAction(nextActiveTextEditor.TextEditorKey));
        // }
        //
        // Dispatcher.Dispatch(new RequestDisposePlainTextEditorAction(TextEditor.TextEditorKey));
    }
    
    private void EditorTabHandleOnKeyDown(KeyboardEventArgs keyboardEventArgs)
    {
        if (KeyboardKeyFacts.WhitespaceCodes.ENTER_CODE == keyboardEventArgs.Code ||
            KeyboardKeyFacts.WhitespaceCodes.SPACE_CODE == keyboardEventArgs.Code)
        {
            DispatchSetActiveTextEditorKeyAction();
        }
    }
}