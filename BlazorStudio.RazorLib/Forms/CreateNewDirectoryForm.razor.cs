using BlazorStudio.ClassLib.FileSystem.Interfaces;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.Forms;

public partial class CreateNewDirectoryForm : ComponentBase
{
    [Parameter]
    public IAbsoluteFilePath ParentDirectory { get; set; } = null!;
    [Parameter]
    public Action<string, string> OnAfterSubmitForm { get; set; } = null!;

    private string _directoryName = String.Empty;

    private void SubmitForm()
    {
        OnAfterSubmitForm(ParentDirectory.GetAbsoluteFilePathString(), _directoryName);
    }
    
    private void DeclineForm()
    {

    }
}