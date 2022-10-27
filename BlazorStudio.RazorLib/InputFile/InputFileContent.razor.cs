using BlazorStudio.ClassLib.Dimensions;
using BlazorStudio.ClassLib.FileSystem.Interfaces;
using BlazorStudio.ClassLib.TreeView;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.InputFile;

public partial class InputFileContent : ComponentBase
{
    [Parameter, EditorRequired]
    public ElementDimensions ContentElementDimensions { get; set; } = null!;
    [Parameter, EditorRequired]
    public Action<TreeViewModel<IAbsoluteFilePath>?> SetSelectedTreeViewModel { get; set; } = null!;
}