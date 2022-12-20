using System.Collections.Immutable;
using BlazorALaCarte.Shared.Dimensions;
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