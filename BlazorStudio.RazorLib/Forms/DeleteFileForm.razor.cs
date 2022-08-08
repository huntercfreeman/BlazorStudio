using BlazorStudio.ClassLib.FileSystem.Interfaces;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.Forms;

public partial class DeleteFileForm : ComponentBase
{
    [Parameter, EditorRequired]
    public IAbsoluteFilePath AbsoluteFilePath { get; set; } = null!;
    [Parameter, EditorRequired]
    public Action<IAbsoluteFilePath> OnAfterSubmitForm { get; set; } = null!;
    [Parameter, EditorRequired]
    public Action OnAfterCancelForm { get; set; } = null!;

    private void SubmitForm()
    {
        OnAfterSubmitForm.Invoke(AbsoluteFilePath);
    }

    private void DeclineForm()
    {
        OnAfterCancelForm.Invoke();
    }
}