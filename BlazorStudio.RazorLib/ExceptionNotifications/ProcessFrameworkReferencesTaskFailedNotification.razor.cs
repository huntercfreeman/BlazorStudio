using BlazorStudio.ClassLib.FileSystem.Interfaces;
using BlazorStudio.ClassLib.Store.NotificationCase;
using BlazorStudio.ClassLib.Store.RoslynWorkspaceState;
using BlazorStudio.ClassLib.Store.SolutionCase;
using Fluxor;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.ExceptionNotifications;

public partial class ProcessFrameworkReferencesTaskFailedNotification : ComponentBase
{
    [Inject]
    private IState<SolutionState> SolutionStateWrap { get; set; } = null!;
    [Inject]
    private IState<RoslynWorkspaceState> RoslynWorkspaceStateWrap { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    [CascadingParameter]
    public NotificationRecord NotificationRecord { get; set; } = null!;

    [Parameter, EditorRequired]
    public IAbsoluteFilePath ProjectAbsoluteFilePath { get; set; } = null!;

    private bool _buttonWasPressed;

    private string GlobalJsonText => $@"
{{
  ""sdk"": {{
    ""version"": ""{RoslynWorkspaceStateWrap.Value?.MsBuildAbsoluteFilePath?.FileNameNoExtension ?? "MsBuildAbsoluteFilePath was null"}""
  }}
}}
";

    private void WriteOutGlobalJsonOnClick()
    {
        if (!_buttonWasPressed)
        {
            _buttonWasPressed = true;

            var containingDirectoryOfProject = (IAbsoluteFilePath)(ProjectAbsoluteFilePath.Directories.Last());

            File.AppendAllText(containingDirectoryOfProject.GetAbsoluteFilePathString() + "global.json",
                GlobalJsonText);

            Dispatcher.Dispatch(new DisposeNotificationAction(NotificationRecord));
        }
    }
}