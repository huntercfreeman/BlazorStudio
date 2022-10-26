using BlazorStudio.ClassLib.Store.DialogCase;
using Fluxor;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.TaskModelManager;

public partial class TaskModelManagerDialogEntryPointDisplay : ComponentBase
{
    private readonly DialogRecord _taskModelManagerDialog = new(
        DialogKey.NewDialogKey(),
        "Task Manager",
        typeof(TaskModelManagerDialogDisplay),
        null
    );

    [Inject]
    private IState<DialogStates> DialogStatesWrap { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    private void OpenTaskModelManagerDialogOnClick()
    {
        if (DialogStatesWrap.Value.List.All(x => x.DialogKey != _taskModelManagerDialog.DialogKey))
            Dispatcher.Dispatch(new RegisterDialogAction(_taskModelManagerDialog));
    }
}