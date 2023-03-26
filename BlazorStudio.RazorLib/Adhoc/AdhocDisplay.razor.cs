using BlazorStudio.ClassLib.BackgroundTaskCase;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.Adhoc;

public partial class AdhocDisplay : ComponentBase, IDisposable
{
    [Inject]
    private IBackgroundTaskQueue BackgroundTaskQueue { get; set; } = null!;
    [Inject]
    private IBackgroundTaskMonitor BackgroundTaskMonitor { get; set; } = null!;

    protected override void OnInitialized()
    {
        BackgroundTaskMonitor.ExecutingBackgroundTaskChanged += BackgroundTaskMonitorOnExecutingBackgroundTaskChanged;
        
        base.OnInitialized();
    }

    private async void BackgroundTaskMonitorOnExecutingBackgroundTaskChanged()
    {
        await InvokeAsync(StateHasChanged);
    }
    
    public void Dispose()
    {
        BackgroundTaskMonitor.ExecutingBackgroundTaskChanged -= BackgroundTaskMonitorOnExecutingBackgroundTaskChanged;
    }
}