using BlazorStudio.ClassLib.Keyboard;
using BlazorStudio.ClassLib.Store.EditorCase;
using BlazorStudio.ClassLib.Store.TextEditorResourceCase;
using BlazorTextEditor.RazorLib;
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
    private IState<TextEditorResourceState> TextEditorResourceStateWrap { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;

    [Parameter]
    [EditorRequired]
    public TextEditorBase TextEditor { get; set; } = null!;
    [Parameter]
    [EditorRequired]
    public int TabIndex { get; set; }

    private string IsActiveCssClass => EditorStateWrap.Value.TextEditorKey == TextEditor.Key
        ? "bstudio_active"
        : string.Empty;

    private void DispatchSetActiveTextEditorKeyAction()
    {
        Dispatcher.Dispatch(
            new SetActiveTextEditorKeyAction(TextEditor.Key));
    }

    private void FireDisposePlainTextEditorOnClick()
    {
        var localTextEditors = TextEditorService.TextEditorStates.TextEditorList;

        var activeIndex = localTextEditors.IndexOf(TextEditor);

        var indexOfNextActiveTextEditor = activeIndex;

        // If last item in list
        if (activeIndex >= localTextEditors.Count - 1)
            indexOfNextActiveTextEditor--;
        else
        {
            // ++ operation because nothing yet has been removed.
            // The new active TextEditor is set prior to actually removing the current active TextEditor.
            indexOfNextActiveTextEditor++;
        }

        // List will be empty after disposing
        if (localTextEditors.Count - 1 == 0)
            Dispatcher.Dispatch(new SetActiveTextEditorKeyAction(TextEditorKey.Empty));
        else
        {
            var nextActiveTextEditor = localTextEditors[indexOfNextActiveTextEditor];

            Dispatcher.Dispatch(new SetActiveTextEditorKeyAction(nextActiveTextEditor.Key));
        }

        TextEditorService.DisposeTextEditor(TextEditor.Key);
    }

    private void EditorTabHandleOnKeyDown(KeyboardEventArgs keyboardEventArgs)
    {
        if (KeyboardKeyFacts.WhitespaceCodes.ENTER_CODE == keyboardEventArgs.Code ||
            KeyboardKeyFacts.WhitespaceCodes.SPACE_CODE == keyboardEventArgs.Code)
            DispatchSetActiveTextEditorKeyAction();
    }
}