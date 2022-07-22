using BlazorStudio.ClassLib.FileSystem.Interfaces;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.Forms;

public partial class CreateNewFileForm : ComponentBase
{
    [Parameter]
    public IAbsoluteFilePath ParentDirectory { get; set; } = null!;
    [Parameter]
    public Action<string, string> OnAfterSubmitForm { get; set; } = null!;

    private string _fileName = String.Empty;

    private void SubmitForm()
    {
        OnAfterSubmitForm(ParentDirectory.GetAbsoluteFilePathString(), _fileName);
    }
    
    private void DeclineForm()
    {

    }
}