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
    private IAbsoluteFilePath _absoluteFilePath = new AbsoluteFilePath("/home/hunter/Documents/TestData/PlainTextEditorStates.Effect.cs", false);

    private readonly SemaphoreSlim _handleAfterOnKeyDownSemaphoreSlim = new(1, 1);
    private string _autoCompleteWordText = string.Empty;

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
            // Example usage:
            // --------------
            var content = await FileSystemProvider.ReadFileAsync(_absoluteFilePath);
            
            var textEditor = new TextEditorBase(
                content,
                new TextEditorCSharpLexer(),
                new TextEditorCSharpDecorationMapper(),
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
        if (!keyboardEventArgs.CtrlKey ||
            keyboardEventArgs.Code != KeyboardKeyFacts.WhitespaceCodes.SPACE_CODE)
        {
            return;
        }
        
        var success = await _handleAfterOnKeyDownSemaphoreSlim
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
            _handleAfterOnKeyDownSemaphoreSlim.Release();
        }
    }
    
    protected override void Dispose(bool disposing)
    {
        TextEditorService.OnTextEditorStatesChanged -= TextEditorServiceOnOnTextEditorStatesChanged;
    
        base.Dispose(disposing);
    }
}