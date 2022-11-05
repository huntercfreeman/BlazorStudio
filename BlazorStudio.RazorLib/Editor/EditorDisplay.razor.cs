using System.Collections.Immutable;
using BlazorStudio.ClassLib.Dimensions;
using BlazorStudio.ClassLib.Keyboard;
using BlazorStudio.ClassLib.Store.EditorCase;
using BlazorStudio.ClassLib.Store.FileSystemCase;
using BlazorStudio.ClassLib.Store.FolderExplorerCase;
using BlazorStudio.ClassLib.Store.InputFileCase;
using BlazorStudio.ClassLib.Store.TextEditorResourceMapCase;
using BlazorTextEditor.RazorLib;
using BlazorTextEditor.RazorLib.Cursor;
using BlazorTextEditor.RazorLib.HelperComponents;
using BlazorTextEditor.RazorLib.Store.TextEditorCase;
using BlazorTextEditor.RazorLib.TextEditor;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorStudio.RazorLib.Editor;

public partial class EditorDisplay : FluxorComponent
{
    [Inject]
    private IState<EditorState> EditorStateWrap { get; set; } = null!;
    [Inject]
    private IState<TextEditorResourceMapState> TextEditorResourceMapStateWrap { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;
    
    [Parameter, EditorRequired]
    public ElementDimensions EditorElementDimensions { get; set; } = null!;
    
    private readonly SemaphoreSlim _afterOnKeyDownSyntaxHighlightingSemaphoreSlim = new(1, 1);
    private TextEditorDisplay? _textEditorDisplay;
    private EditorTabsDisplay? _editorTabsDisplay;

    private async Task HandleAfterOnKeyDownAsync(
        TextEditorBase textEditor,
        ImmutableArray<TextEditorCursorSnapshot> cursorSnapshots,
        KeyboardEventArgs keyboardEventArgs,
        Func<TextEditorMenuKind, bool, Task> setTextEditorMenuKind)
    {
        var primaryCursorSnapshot = cursorSnapshots
            .First(x =>
                x.UserCursor.IsPrimaryCursor);

        // Update the * for all the tabs
        // that shows if it has been 'modified'.
        //
        // TODO: Change the logic for showing * such that
        // it more accurately represents a file having been modified.
        //
        // This is just saying on key press presume it was modified.
        if (_editorTabsDisplay is not null)
            _editorTabsDisplay.ForceReloadTabs();

        if (keyboardEventArgs.Key == ";" ||
            KeyboardKeyFacts.IsWhitespaceCode(keyboardEventArgs.Code) ||
            (keyboardEventArgs.CtrlKey && keyboardEventArgs.Key == "v"))
        {
            // Syntax Highlighting

            var success = await _afterOnKeyDownSyntaxHighlightingSemaphoreSlim
                .WaitAsync(TimeSpan.Zero);

            if (!success)
                return;

            try
            {
                await textEditor.ApplySyntaxHighlightingAsync();

                await InvokeAsync(StateHasChanged);
            }
            finally
            {
                _afterOnKeyDownSyntaxHighlightingSemaphoreSlim.Release();
            }
        }
    }

    private void HandleOnSaveRequested(TextEditorBase textEditor)
    {
        var content = textEditor.GetAllText();
        
        var textEditorResourceMapState = TextEditorResourceMapStateWrap.Value;
        
        _ = textEditorResourceMapState.ResourceMap
            .TryGetValue(
                textEditor.Key, 
                out var resource);

        var saveFileAction = new FileSystemState.SaveFileAction(
            resource,
            content);
        
        Dispatcher.Dispatch(saveFileAction);
        
        textEditor.ClearEditBlocks();
    }
}