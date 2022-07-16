using Fluxor;

namespace BlazorStudio.ClassLib.Store.DialogCase;

public class DialogStatesReducer
{
    [ReducerMethod]
    public static DialogStates ReduceRegisterDialogAction(DialogStates previousDialogStates,
        RegisterDialogAction registerDialogAction)
    {
        return new DialogStates(previousDialogStates.List
            .Add(registerDialogAction.DialogRecord));
    }
    
    [ReducerMethod]
    public static DialogStates ReduceDisposeDialogAction(DialogStates previousDialogStates,
        DisposeDialogAction disposeDialogAction)
    {
        return new DialogStates(previousDialogStates.List
            .Remove(disposeDialogAction.DialogRecord));
    }
}