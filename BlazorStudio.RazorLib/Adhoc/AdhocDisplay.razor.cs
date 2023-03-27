using BlazorStudio.ClassLib.BackgroundTaskCase;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.Adhoc;

public partial class AdhocDisplay : ComponentBase, IDisposable
{
    [Inject]
    private IBackgroundTaskQueue BackgroundTaskQueue { get; set; } = null!;
    [Inject]
    private IBackgroundTaskMonitor BackgroundTaskMonitor { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    protected override void OnInitialized()
    {
        BackgroundTaskMonitor.ExecutingBackgroundTaskChanged += BackgroundTaskMonitorOnExecutingBackgroundTaskChanged;
        
        base.OnInitialized();
    }

    private async void BackgroundTaskMonitorOnExecutingBackgroundTaskChanged()
    {
        await InvokeAsync(StateHasChanged);
    }
    
    private void EnqueueTaskOnClick()
    {
        var backgroundTask = new BackgroundTask(
            async cancellationToken => await Task.Delay(1_500, cancellationToken),
            "Aaa",
            "Bbb",
            cancellationToken => Task.CompletedTask,
            Dispatcher,
            BackgroundTaskKey.NewBackgroundTaskKey());
        
        BackgroundTaskQueue.QueueBackgroundWorkItem(
            backgroundTask);
    }
    
    public void Dispose()
    {
        BackgroundTaskMonitor.ExecutingBackgroundTaskChanged -= BackgroundTaskMonitorOnExecutingBackgroundTaskChanged;
    }
}