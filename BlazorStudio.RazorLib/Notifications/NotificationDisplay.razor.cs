using BlazorStudio.ClassLib.Store.NotificationCase;
using Fluxor;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.Notifications;

public partial class NotificationDisplay : ComponentBase, IDisposable
{
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;
    
    [Parameter, EditorRequired]
    public NotificationRecord NotificationRecord { get; set; } = null!;

    private readonly CancellationTokenSource _notificationOverlayCancellationTokenSource = new();
    private readonly TimeSpan _notificationOverlayLifespan = TimeSpan.FromSeconds(5);
    
    protected override Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            _ = Task.Run(async () =>
            {
                await Task.Delay(
                    _notificationOverlayLifespan,
                    _notificationOverlayCancellationTokenSource.Token);
                
                Dispatcher.Dispatch(
                    new NotificationState.DisposeNotificationAction(
                        NotificationRecord.NotificationKey));
            });
        }
        
        return base.OnAfterRenderAsync(firstRender);
    }

    public void Dispose()
    {
        _notificationOverlayCancellationTokenSource.Cancel();
    }
}