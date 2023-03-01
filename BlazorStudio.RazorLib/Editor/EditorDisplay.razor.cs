using System.Collections.Immutable;
using BlazorALaCarte.Shared.Dimensions;
using BlazorCommon.RazorLib.Dimensions;
using BlazorStudio.ClassLib.Store.EditorCase;
using BlazorStudio.ClassLib.Store.FileSystemCase;
using BlazorStudio.ClassLib.Store.FolderExplorerCase;
using BlazorStudio.ClassLib.Store.InputFileCase;
using BlazorTextEditor.RazorLib;
using BlazorTextEditor.RazorLib.Cursor;
using BlazorTextEditor.RazorLib.HelperComponents;
using BlazorTextEditor.RazorLib.ViewModel;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorStudio.RazorLib.Editor;

public partial class EditorDisplay : FluxorComponent
{
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;
    
    [Parameter, EditorRequired]
    public ElementDimensions EditorElementDimensions { get; set; } = null!;
}