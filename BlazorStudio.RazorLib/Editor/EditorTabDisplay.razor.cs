using BlazorStudio.ClassLib.Keyboard;
using BlazorStudio.ClassLib.Store.EditorCase;
using BlazorStudio.ClassLib.Store.TextEditorCase;
using BlazorStudio.ClassLib.TextEditor;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorStudio.RazorLib.Editor;

public partial class EditorTabDisplay : FluxorComponent
{
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;
    [Inject]
    private IState<EditorState> EditorStateWrap { get; set; } = null!;
    
    [Parameter, EditorRequired]
    public TextEditorBase TextEditor { get; set; } = null!;
    [Parameter, EditorRequired]
    public int TabIndex { get; set; }
    
    private string IsActiveCssClass => EditorStateWrap.Value.TextEditorKey == TextEditor.TextEditorKey
        ? "bstudio_active"
        : string.Empty;

    private void DispatchSetActiveTextEditorKeyAction()
    {
        Dispatcher.Dispatch(new SetActiveTextEditorKeyAction(TextEditor.TextEditorKey));
    }
    
    private void FireDisposePlainTextEditorOnClick()
    {
        Dispatcher.Dispatch(new RequestDisposePlainTextEditorAction(TextEditor.TextEditorKey));
        Dispatcher.Dispatch(new SetActiveTextEditorKeyAction(TextEditor.TextEditorKey));
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