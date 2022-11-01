using System.Text;
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
    [Parameter, EditorRequired]
    public int Index { get; set; }

    private const int WIDTH_IN_PIXELS = 100;
    private const int HEIGHT_IN_PIXELS = 100;
    private const int RIGHT_OFFSET_IN_PIXELS = 15;
    private const int BOTTOM_OFFSET_IN_PIXELS = 15;
    private const int MARGIN_TOP_IN_PIXELS = 15;
    
    private readonly CancellationTokenSource _notificationOverlayCancellationTokenSource = new();
    private readonly TimeSpan _notificationOverlayLifespan = TimeSpan.FromSeconds(5);

    private string CssStyleString => GetCssStyleString();

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

    private string GetCssStyleString()
    {
        var styleBuilder = new StringBuilder();

        styleBuilder.Append($" width: {WIDTH_IN_PIXELS}px;");
        styleBuilder.Append($" height: {HEIGHT_IN_PIXELS}px;");
        
        styleBuilder.Append($" right: {RIGHT_OFFSET_IN_PIXELS}px;");

        var bottomOffsetDueToHeight = HEIGHT_IN_PIXELS * Index;
        var bottomOffsetDueToMarginTop = MARGIN_TOP_IN_PIXELS * Index;
        var bottomOffsetDueToBottomOffset = BOTTOM_OFFSET_IN_PIXELS * (1 + Index);
        
        var totalBottomOffset = bottomOffsetDueToHeight +
                                bottomOffsetDueToMarginTop +
                                bottomOffsetDueToBottomOffset;
        
        styleBuilder.Append($" bottom: {totalBottomOffset}px;");

        return styleBuilder.ToString();
    }
    
    public void Dispose()
    {
        _notificationOverlayCancellationTokenSource.Cancel();
    }
}