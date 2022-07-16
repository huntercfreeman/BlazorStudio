using BlazorStudio.ClassLib.TaskModelManager;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.TaskModelManager.Helpers;

public partial class TaskModelDisplay : ComponentBase
{
    [Parameter]
    public ITaskModel TaskModel { get; set; } = null!;
    [Parameter]
    public bool ShowCancelButton { get; set; }

    private void CancelTaskOnClick()
    {
        TaskModel.CancellationTokenSource.Cancel();

        StateHasChanged();
    }
}