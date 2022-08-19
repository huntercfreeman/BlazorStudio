using BlazorStudio.ClassLib.FileSystem.Interfaces;
using BlazorStudio.ClassLib.Keyboard;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorStudio.RazorLib.Forms;

public partial class DeleteFileForm : ComponentBase
{
    [Parameter, EditorRequired]
    public IAbsoluteFilePath AbsoluteFilePath { get; set; } = null!;
    [Parameter, EditorRequired]
    public Action<IAbsoluteFilePath> OnAfterSubmitForm { get; set; } = null!;
    [Parameter, EditorRequired]
    public Action OnAfterCancelForm { get; set; } = null!;

    private bool _isLoaded;
    private int _directDescendantCount;

    protected override Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            if (AbsoluteFilePath.IsDirectory)
            {
                var childFileSystemEntries = Directory.GetFileSystemEntries(AbsoluteFilePath.GetAbsoluteFilePathString());

                _directDescendantCount = childFileSystemEntries.Length;
            }

            _isLoaded = true;
            InvokeAsync(StateHasChanged);
        }

        return base.OnAfterRenderAsync(firstRender);
    }

    private void SubmitForm()
    {
        OnAfterSubmitForm.Invoke(AbsoluteFilePath);
    }

    private void DeclineForm()
    {
        OnAfterCancelForm.Invoke();
    }
}