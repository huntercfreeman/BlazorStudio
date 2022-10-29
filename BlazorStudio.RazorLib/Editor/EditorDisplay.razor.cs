using BlazorStudio.ClassLib.Dimensions;
using BlazorStudio.ClassLib.Store.EditorCase;
using BlazorStudio.ClassLib.Store.FolderExplorerCase;
using BlazorStudio.ClassLib.Store.InputFileCase;
using BlazorTextEditor.RazorLib;
using BlazorTextEditor.RazorLib.Store.TextEditorCase;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.Editor;

public partial class EditorDisplay : FluxorComponent
{
    [Inject]
    private IState<EditorState> EditorStateWrap { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;
    
    [Parameter, EditorRequired]
    public ElementDimensions EditorElementDimensions { get; set; } = null!;

    private Task OpenFileOnClick()
    {
        Dispatcher.Dispatch(
            new InputFileState.RequestInputFileStateFormAction(
                afp =>
                {
                    Dispatcher.Dispatch(
                        new SetFolderExplorerStateAction(afp));
                    
                    return Task.CompletedTask;
                },
                afp =>
                {
                    if (afp is null ||
                        afp.IsDirectory)
                    {
                        return Task.FromResult(false);
                    }
                    
                    return Task.FromResult(true);
                }));
        
        return Task.CompletedTask;
    }
}