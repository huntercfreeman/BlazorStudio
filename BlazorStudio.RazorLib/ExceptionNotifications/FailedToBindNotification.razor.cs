using BlazorStudio.ClassLib.FileSystem.Interfaces;
using BlazorStudio.ClassLib.Store.NotificationCase;
using Microsoft.AspNetCore.Components;
using System.Diagnostics;

namespace BlazorStudio.RazorLib.ExceptionNotifications;

public partial class FailedToBindNotification : ComponentBase
{
    [CascadingParameter]
    public NotificationRecord NotificationRecord { get; set; } = null!;

    [Parameter, EditorRequired]
    public IAbsoluteFilePath ProjectAbsoluteFilePath { get; set; } = null!;

    protected override Task OnAfterRenderAsync(bool firstRender)
    {
        //var command = $"netstat -ano | findStr \"{port}\"";

        //var command = $"tasklist /fi \"pid eq {processId}\"";

        //var command = $"taskkill /PID {processId} /F";

        return base.OnAfterRenderAsync(firstRender);
    }
}