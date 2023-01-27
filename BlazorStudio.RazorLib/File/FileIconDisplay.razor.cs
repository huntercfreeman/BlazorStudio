using BlazorStudio.ClassLib.FileSystem.Interfaces;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.File;

public partial class FileIconDisplay : ComponentBase
{
    [Parameter, EditorRequired]
    public IAbsoluteFilePath AbsoluteFilePath { get; set; } = null!;
}