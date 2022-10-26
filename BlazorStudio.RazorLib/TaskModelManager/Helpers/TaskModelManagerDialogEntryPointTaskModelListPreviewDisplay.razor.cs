using System.Collections.Immutable;
using BlazorStudio.ClassLib.TaskModelManager;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.TaskModelManager.Helpers;

public partial class TaskModelManagerDialogEntryPointTaskModelListPreviewDisplay : ComponentBase, IDisposable
{
    private ImmutableArray<ITaskModel> _taskModelCache = ImmutableArray<ITaskModel>.Empty;
    [Parameter]
    public Func<IEnumerable<ITaskModel>> GetTaskModelsFunc { get; set; } = null!;
    [Parameter]
    public string ListTitle { get; set; } = null!;
    [Parameter]
    public string ShortListTitle { get; set; } = null!;
    [Parameter]
    public string ClassParameter { get; set; } = string.Empty;
    [Parameter]
    public string StyleParameter { get; set; } = string.Empty;

    public void Dispose()
    {
        TaskModelManagerService.OnTasksStateHasChangedEventHandler -=
            TaskModelManagerServiceStateHasChangedEventHandler;
        TaskModelManagerService.OnTaskSeemsUnresponsiveEventHandler -=
            TaskModelManagerServiceStateHasChangedEventHandler;
    }

    protected override void OnInitialized()
    {
        _taskModelCache = GetTaskModelsFunc().ToImmutableArray();

        TaskModelManagerService.OnTasksStateHasChangedEventHandler +=
            TaskModelManagerServiceStateHasChangedEventHandler;
        TaskModelManagerService.OnTaskSeemsUnresponsiveEventHandler +=
            TaskModelManagerServiceStateHasChangedEventHandler;

        base.OnInitialized();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            _taskModelCache = GetTaskModelsFunc().ToImmutableArray();
            await InvokeAsync(StateHasChanged);
        }

        await base.OnAfterRenderAsync(firstRender);
    }

    private async void TaskModelManagerServiceStateHasChangedEventHandler(object? sender, EventArgs? e)
    {
        _taskModelCache = GetTaskModelsFunc().ToImmutableArray();
        await InvokeAsync(StateHasChanged);
    }
}