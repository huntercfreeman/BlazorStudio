using BlazorStudio.ClassLib.FileSystem.Interfaces;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.Forms;

public partial class DeleteFileForm : ComponentBase
{
    private int _directDescendantCount;

    private bool _isLoaded;
    [Parameter]
    [EditorRequired]
    public IAbsoluteFilePath AbsoluteFilePath { get; set; } = null!;
    [Parameter]
    [EditorRequired]
    public Action<IAbsoluteFilePath> OnAfterSubmitForm { get; set; } = null!;
    [Parameter]
    [EditorRequired]
    public Action OnAfterCancelForm { get; set; } = null!;

    protected override Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            if (AbsoluteFilePath.IsDirectory)
            {
                var childFileSystemEntries =
                    Directory.GetFileSystemEntries(AbsoluteFilePath.GetAbsoluteFilePathString());

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