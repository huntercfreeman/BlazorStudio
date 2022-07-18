using BlazorStudio.ClassLib.FileSystem.Interfaces;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.Forms;

public partial class CreateNewFileForm : ComponentBase
{
    [Parameter]
    public IAbsoluteFilePath ParentDirectory { get; set; } = null!;
}