using Fluxor;

namespace BlazorStudio.ClassLib.Store.DialogCase;

public class DialogStatesReducer
{
    [ReducerMethod]
    public static DialogStates ReduceRegisterDialogRecordAction(DialogStates previousDialogStates,
        RegisterDialogRecordAction registerDialogRecordAction)
    {
        if (previousDialogStates.DialogRecords
            .Any(x => x.DialogKey == registerDialogRecordAction.DialogRecord.DialogKey))
        {
            return previousDialogStates;
        }
        
        var nextList = previousDialogStates.DialogRecords
            .Add(registerDialogRecordAction.DialogRecord);

        return previousDialogStates with
        {
            DialogRecords = nextList
        };
    }
    
    [ReducerMethod]
    public static DialogStates ReduceDisposeDialogRecordAction(DialogStates previousDialogStates,
        DisposeDialogRecordAction disposeDialogRecordAction)
    {
        var nextList = previousDialogStates.DialogRecords
            .Remove(disposeDialogRecordAction.DialogRecord);

        return previousDialogStates with
        {
            DialogRecords = nextList
        };
    }
}