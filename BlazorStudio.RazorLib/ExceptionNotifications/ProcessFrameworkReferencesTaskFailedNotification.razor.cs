using BlazorStudio.ClassLib.FileSystem.Interfaces;
using BlazorStudio.ClassLib.Store.NotificationCase;
using BlazorStudio.ClassLib.Store.SolutionCase;
using Fluxor;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.ExceptionNotifications;

public partial class ProcessFrameworkReferencesTaskFailedNotification : ComponentBase
{
    [Inject]
    private IState<SolutionState> SolutionStateWrap { get; set; } = null!;

    [CascadingParameter]
    public NotificationRecord NotificationRecord { get; set; } = null!;

    [Parameter, EditorRequired]
    public IAbsoluteFilePath ProjectAbsoluteFilePath { get; set; } = null!;
}