using BlazorStudio.ClassLib.Store.NotificationCase;
using Fluxor;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.Notification;

public partial class NotificationDisplay : ComponentBase
{
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    [Parameter, EditorRequired]
    public NotificationRecord NotificationRecord { get; set; } = null!;

    protected override Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            if (NotificationRecord.AutomaticDisposalTimeSpan is not null)
            {
                Task.Run(async () =>
                {
                    await Task.Delay(NotificationRecord.AutomaticDisposalTimeSpan.Value);

                    Dismiss();
                });
            }
        }
        
        return base.OnAfterRenderAsync(firstRender);
    }

    private void Dismiss()
    {
        Dispatcher.Dispatch(new DisposeNotificationAction(NotificationRecord));
    }
    
    private void DismissOnClick()
    {
        Dismiss();
    }
}