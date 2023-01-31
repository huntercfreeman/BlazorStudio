using BlazorALaCarte.Shared.Dimensions;
using BlazorALaCarte.TreeView;
using BlazorALaCarte.TreeView.Services;
using BlazorStudio.ClassLib.CommonComponents;
using BlazorStudio.ClassLib.FileSystem.Classes;
using BlazorStudio.ClassLib.FileSystem.Interfaces;
using BlazorStudio.ClassLib.Store.InputFileCase;
using BlazorStudio.RazorLib.InputFile.Classes;
using Fluxor;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.InputFile.InternalComponents;

public partial class InputFileContent : ComponentBase
{
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;
    [Inject]
    private ClassLib.Menu.ICommonMenuOptionsFactory CommonMenuOptionsFactory { get; set; } = null!;
    [Inject]
    private ITreeViewService TreeViewService { get; set; } = null!;
    [Inject]
    private ICommonComponentRenderers CommonComponentRenderers { get; set; } = null!;

    [CascadingParameter(Name = "SetInputFileContentTreeViewRoot")]
    public Action<IAbsoluteFilePath> SetInputFileContentTreeViewRoot { get; set; } = null!;
    [CascadingParameter]
    public InputFileTreeViewMouseEventHandler InputFileTreeViewMouseEventHandler { get; set; } = null!;
    [CascadingParameter]
    public InputFileTreeViewKeyboardEventHandler InputFileTreeViewKeyboardEventHandler { get; set; } = null!;
    [CascadingParameter]
    public InputFileState InputFileState { get; set; } = null!;
    
    [Parameter, EditorRequired]
    public ElementDimensions ElementDimensions { get; set; } = null!;
    [Parameter, EditorRequired]
    public Action<IAbsoluteFilePath?> SetSelectedAbsoluteFilePath { get; set; } = null!;
    
    public static readonly TreeViewStateKey TreeViewInputFileContentStateKey = 
        TreeViewStateKey.NewTreeViewStateKey();

    protected override void OnInitialized()
    {
        if (!TreeViewService.TryGetTreeViewState(
                TreeViewInputFileContentStateKey, 
                out _))
        {
            var directoryHomeAbsoluteFilePath = new AbsoluteFilePath(
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                true);
    
            SetInputFileContentTreeViewRoot.Invoke(directoryHomeAbsoluteFilePath);
        }
        
        base.OnInitialized();
    }
}