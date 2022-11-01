using Fluxor;

namespace BlazorStudio.ClassLib.Store.ProgramExecutionCase;

public class ProgramExecutionStateReducer
{
    [ReducerMethod]
    public static ProgramExecutionState ReduceSetStartupProjectAbsoluteFilePathAction(
        ProgramExecutionState inProgramExecutionState,
        ProgramExecutionState.SetStartupProjectAbsoluteFilePathAction setStartupProjectAbsoluteFilePathAction)
    {
        return inProgramExecutionState with
        {
            StartupProjectAbsoluteFilePath = 
                setStartupProjectAbsoluteFilePathAction.StartupProjectAbsoluteFilePath
        };
    }
}