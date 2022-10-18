using BlazorStudio.ClassLib.Contexts;
using BlazorStudio.ClassLib.FileSystem.Classes;
using BlazorStudio.ClassLib.FileSystem.Interfaces;
using BlazorStudio.ClassLib.Store.ContextCase;
using BlazorStudio.ClassLib.Store.EditorCase;
using BlazorStudio.ClassLib.SyntaxHighlighting;
using BlazorStudio.ClassLib.UserInterface;
using BlazorStudio.RazorLib.ContextCase;
using BlazorTextEditor.RazorLib;
using BlazorTextEditor.RazorLib.TextEditor;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.Editor;

public partial class EditorDisplay : FluxorComponent
{
    [Inject]
    private IState<EditorState> EditorStateWrap { get; set; } = null!;
    [Inject]
    private IFileSystemProvider FileSystemProvider { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;

    [Parameter, EditorRequired]
    public ClassLib.UserInterface.Dimensions Dimensions { get; set; } = null!;

    private TextEditorKey _testTextEditorKey = TextEditorKey.NewTextEditorKey();
    private IAbsoluteFilePath _absoluteFilePath = new AbsoluteFilePath("/home/hunter/Documents/TestData/PlainTextEditorStates.Effect.cs", false);

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
            var content = await FileSystemProvider.ReadFileAsync(_absoluteFilePath);

            var textEditor = new TextEditorBase(
                content,
                new TextEditorCSharpLexer(),
                new TextEditorCSharpDecorationMapper(),
                _testTextEditorKey);

            await textEditor.ApplySyntaxHighlightingAsync();
            
            TextEditorService
                .RegisterTextEditor(textEditor);
        }
        
        await base.OnAfterRenderAsync(firstRender);
    }

    private void TextEditorServiceOnOnTextEditorStatesChanged(object? sender, EventArgs e)
    {
        InvokeAsync(StateHasChanged);
    }
    
    protected override void Dispose(bool disposing)
    {
        TextEditorService.OnTextEditorStatesChanged -= TextEditorServiceOnOnTextEditorStatesChanged;
    
        base.Dispose(disposing);
    }
}