using System.Collections.Immutable;
using BlazorALaCarte.Shared.Dimensions;
using BlazorALaCarte.Shared.Menu;
using BlazorALaCarte.TreeView;
using BlazorALaCarte.TreeView.Events;
using BlazorALaCarte.TreeView.Services;
using BlazorStudio.ClassLib.CommonComponents;
using BlazorStudio.ClassLib.Dimensions;
using BlazorStudio.ClassLib.FileSystem.Classes;
using BlazorStudio.ClassLib.FileSystem.Interfaces;
using BlazorStudio.ClassLib.Menu;
using BlazorStudio.ClassLib.Namespaces;
using BlazorStudio.ClassLib.Store.InputFileCase;
using BlazorStudio.ClassLib.TreeViewImplementations;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorStudio.RazorLib.InputFile;

public partial class InputFileContent : FluxorComponent
{
    [Inject]
    private IState<InputFileState> InputFileStateWrap { get; set; } = null!;
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
    public TreeViewMouseEventRegistrar TreeViewMouseEventRegistrar { get; set; } = null!;
    [CascadingParameter]
    public InputFileTreeViewKeymap InputFileTreeViewKeymap { get; set; } = null!;
    
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
                out var treeViewState))
        {
            var directoryHomeAbsoluteFilePath = new AbsoluteFilePath(
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                true);

            SetInputFileContentTreeViewRoot.Invoke(directoryHomeAbsoluteFilePath);
        }
        
        base.OnInitialized();
    }
}