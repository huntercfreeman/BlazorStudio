using Blazor.Text.Editor.Analysis.Html.ClassLib;
using Blazor.Text.Editor.Analysis.Razor.ClassLib;
using BlazorStudio.ClassLib.Contexts;
using BlazorStudio.ClassLib.FileSystem.Classes;
using BlazorStudio.ClassLib.FileSystem.Interfaces;
using BlazorStudio.ClassLib.Renderer;
using BlazorStudio.ClassLib.Store.ContextCase;
using BlazorStudio.ClassLib.Store.EditorCase;
using BlazorStudio.ClassLib.Store.FileSystemCase;
using BlazorStudio.ClassLib.Store.NotificationCase;
using BlazorStudio.ClassLib.Store.TextEditorResourceCase;
using BlazorStudio.ClassLib.SyntaxHighlighting;
using BlazorStudio.ClassLib.UserInterface;
using BlazorStudio.RazorLib.ContextCase;
using BlazorTextEditor.RazorLib;
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

    [Parameter, EditorRequired]
    public ClassLib.UserInterface.Dimensions Dimensions { get; set; } = null!;

    private TextEditorKey _testTextEditorKey = TextEditorKey.NewTextEditorKey();
    private IAbsoluteFilePath _absoluteFilePath = new AbsoluteFilePath(
        @"/home/hunter/Documents/TestData/blazorCounter.razor",
        false);

    private readonly SemaphoreSlim _afterOnKeyDownAutoCompleteSemaphoreSlim = new(1, 1);
    private readonly SemaphoreSlim _afterOnKeyDownSyntaxHighlightingSemaphoreSlim = new(1, 1);
    private string _autoCompleteWordText = string.Empty;
    private string _previousDimensionsCssString = string.Empty;

    private bool _textEditorShouldRemeasureFlag;
    private TextEditorDisplay? _textEditorDisplay;

    private TextEditorBase? TestTextEditor => TextEditorService.TextEditorStates.TextEditorList
        .FirstOrDefault(x => x.Key == _testTextEditorKey);
    
    protected override Task OnInitializedAsync()
    {
        TextEditorService.OnTextEditorStatesChanged += TextEditorServiceOnOnTextEditorStatesChanged;
        
        return base.OnInitializedAsync();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            _previousDimensionsCssString = Dimensions.DimensionsCssString;
            
            // Example usage:
            // --------------
            var content = await FileSystemProvider.ReadFileAsync(_absoluteFilePath);
            
            var textEditor = new TextEditorBase(
                content,
                new TextEditorRazorLexer(),
                new TextEditorHtmlDecorationMapper(),
                _testTextEditorKey);
            
            await textEditor.ApplySyntaxHighlightingAsync();
            
            TextEditorService
                .RegisterTextEditor(textEditor);
            
            Dispatcher.Dispatch(
                new SetActiveTextEditorKeyAction(_testTextEditorKey));
        }

        await base.OnAfterRenderAsync(firstRender);
    }

    private void TextEditorServiceOnOnTextEditorStatesChanged(object? sender, EventArgs e)
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
                $"Could not find resource file",
                DefaultErrorRenderer.GetType(),
                null,
                TimeSpan.FromSeconds(3))));
        }
    }
    
    private async Task HandleAfterOnKeyDownAsync(
        TextEditorBase textEditor, 
        ImmutableTextEditorCursor immutablePrimaryCursor, 
        KeyboardEventArgs keyboardEventArgs,
        Func<TextEditorMenuKind, Task> setTextEditorMenuKind)
    {
        if (keyboardEventArgs.CtrlKey &&
            keyboardEventArgs.Code == KeyboardKeyFacts.WhitespaceCodes.SPACE_CODE ||
            // My recording software blocks Ctrl + Space keybind I need
            // to find time to look into how to fix this but for now I added Alt + a
            keyboardEventArgs.AltKey &&
            keyboardEventArgs.Key == "a")
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
                        immutablePrimaryCursor.RowIndex,
                        immutablePrimaryCursor.ColumnIndex,
                        true);

                // word: meaning any contiguous section of RichCharacters of the same kind
                var startOfWord = columnIndexOfCharacterWithDifferingKind == -1
                    ? columnIndexOfCharacterWithDifferingKind + 1
                    : columnIndexOfCharacterWithDifferingKind;

                var positionIndex = textEditor.GetPositionIndex(
                    immutablePrimaryCursor.RowIndex,
                    startOfWord);
            
                _autoCompleteWordText = textEditor.GetTextRange(
                    positionIndex,
                    immutablePrimaryCursor.ColumnIndex - startOfWord);

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
            
            return;
        }
    }
    
    protected override void Dispose(bool disposing)
    {
        TextEditorService.OnTextEditorStatesChanged -= TextEditorServiceOnOnTextEditorStatesChanged;
    
        base.Dispose(disposing);
    }
}