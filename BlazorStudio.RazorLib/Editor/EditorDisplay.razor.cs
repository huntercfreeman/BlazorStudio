using System.Collections.Immutable;
using BlazorStudio.ClassLib.FileSystem.Classes;
using BlazorStudio.ClassLib.FileSystem.Interfaces;
using BlazorStudio.ClassLib.Renderer;
using BlazorStudio.ClassLib.Store.EditorCase;
using BlazorStudio.ClassLib.Store.FileSystemCase;
using BlazorStudio.ClassLib.Store.NotificationCase;
using BlazorStudio.ClassLib.Store.TextEditorResourceCase;
using BlazorTextEditor.RazorLib;
using BlazorTextEditor.RazorLib.Cursor;
using BlazorTextEditor.RazorLib.HelperComponents;
using BlazorTextEditor.RazorLib.Keyboard;
using BlazorTextEditor.RazorLib.TextEditor;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorStudio.RazorLib.Editor;

public partial class EditorDisplay : FluxorComponent
{
    private readonly SemaphoreSlim _afterOnKeyDownAutoCompleteSemaphoreSlim = new(1, 1);
    private readonly SemaphoreSlim _afterOnKeyDownSyntaxHighlightingSemaphoreSlim = new(1, 1);

    private IAbsoluteFilePath _absoluteFilePath = new AbsoluteFilePath(
        @"C:\Users\hunte\source\Razor\Razor.ServerSide\Pages\Counter.razor",
        false);

    private string _autoCompleteWordText = string.Empty;
    private string _previousDimensionsCssString = string.Empty;

    private TextEditorKey _testTextEditorKey = TextEditorKey.NewTextEditorKey();
    private TextEditorDisplay? _textEditorDisplay;

    private bool _textEditorShouldRemeasureFlag;
    [Inject]
    private IState<EditorState> EditorStateWrap { get; set; } = null!;
    [Inject]
    private IState<TextEditorResourceState> TextEditorResourceStateWrap { get; set; } = null!;
    [Inject]
    private IFileSystemProvider FileSystemProvider { get; set; } = null!;
    [Inject]
    private IDefaultErrorRenderer DefaultErrorRenderer { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;

    [Parameter]
    [EditorRequired]
    public Dimensions Dimensions { get; set; } = null!;

    private TextEditorBase? TestTextEditor => TextEditorService.TextEditorStates.TextEditorList
        .FirstOrDefault(x => x.Key == _testTextEditorKey);

    protected override Task OnInitializedAsync()
    {
        TextEditorService.OnTextEditorStatesChanged += TextEditorServiceOnOnTextEditorStatesChanged;

        return base.OnInitializedAsync();
    }

    private void TextEditorServiceOnOnTextEditorStatesChanged()
    {
        InvokeAsync(StateHasChanged);
    }

    private void HandleOnSaveRequested(TextEditorBase textEditor)
    {
        textEditor.ClearEditBlocks();

        var content = textEditor.GetAllText();

        var textEditorResourceState = TextEditorResourceStateWrap.Value;

        if (textEditorResourceState.ResourceMap
            .TryGetValue(textEditor.Key, out var relatedResource))
        {
            Dispatcher.Dispatch(
                new WriteToFileSystemAction(
                    relatedResource,
                    content));
        }
        else
        {
            Dispatcher.Dispatch(new RegisterNotificationAction(new NotificationRecord(
                NotificationKey.NewNotificationKey(),
                "Could not find resource file",
                DefaultErrorRenderer.GetType(),
                null,
                TimeSpan.FromSeconds(3))));
        }
    }

    private async Task HandleAfterOnKeyDownAsync(
        TextEditorBase textEditor,
        ImmutableArray<TextEditorCursorSnapshot> cursorSnapshots,
        KeyboardEventArgs keyboardEventArgs,
        Func<TextEditorMenuKind, Task> setTextEditorMenuKind)
    {
        var primaryCursorSnapshot = cursorSnapshots
            .First(x =>
                x.UserCursor.IsPrimaryCursor);

        if ((keyboardEventArgs.CtrlKey &&
             keyboardEventArgs.Code == KeyboardKeyFacts.WhitespaceCodes.SPACE_CODE) ||
            // My recording software blocks Ctrl + Space keybind I need
            // to find time to look into how to fix this but for now I added Alt + a
            (keyboardEventArgs.AltKey &&
             keyboardEventArgs.Key == "a"))
        {
            // AutoComplete

            var success = await _afterOnKeyDownAutoCompleteSemaphoreSlim
                .WaitAsync(TimeSpan.Zero);

            if (!success)
                return;

            try
            {
                var columnIndexOfCharacterWithDifferingKind = textEditor
                    .GetColumnIndexOfCharacterWithDifferingKind(
                        primaryCursorSnapshot.ImmutableCursor.RowIndex,
                        primaryCursorSnapshot.ImmutableCursor.ColumnIndex,
                        true);

                // word: meaning any contiguous section of RichCharacters of the same kind
                var startOfWord = columnIndexOfCharacterWithDifferingKind == -1
                    ? columnIndexOfCharacterWithDifferingKind + 1
                    : columnIndexOfCharacterWithDifferingKind;

                var positionIndex = textEditor.GetPositionIndex(
                    primaryCursorSnapshot.ImmutableCursor.RowIndex,
                    startOfWord);

                _autoCompleteWordText = textEditor.GetTextRange(
                    positionIndex,
                    primaryCursorSnapshot.ImmutableCursor.ColumnIndex - startOfWord);

                await setTextEditorMenuKind.Invoke(TextEditorMenuKind.AutoCompleteMenu);
            }
            finally
            {
                _afterOnKeyDownAutoCompleteSemaphoreSlim.Release();
            }
        }
        else if (keyboardEventArgs.Key == ";" ||
                 KeyboardKeyFacts.IsWhitespaceCode(keyboardEventArgs.Code))
        {
            // Syntax Highlighting

            var success = await _afterOnKeyDownSyntaxHighlightingSemaphoreSlim
                .WaitAsync(TimeSpan.Zero);

            if (!success)
                return;

            try
            {
                await textEditor.ApplySyntaxHighlightingAsync();
            }
            finally
            {
                _afterOnKeyDownSyntaxHighlightingSemaphoreSlim.Release();
            }
        }
    }

    protected override void Dispose(bool disposing)
    {
        TextEditorService.OnTextEditorStatesChanged -= TextEditorServiceOnOnTextEditorStatesChanged;

        base.Dispose(disposing);
    }
}