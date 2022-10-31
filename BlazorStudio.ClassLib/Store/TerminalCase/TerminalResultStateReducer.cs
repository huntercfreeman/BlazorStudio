using Fluxor;

namespace BlazorStudio.ClassLib.Store.TerminalCase;

public class TerminalResultStateReducer
{
    [ReducerMethod]
    public static TerminalResultState ReduceRegisterTerminalResultAction(
        TerminalResultState inTerminalResultState,
        RegisterTerminalResultAction registerTerminalResultAction)
    {
        var nextResultMap = inTerminalResultState.TerminalCommandResultMap
            .Add(
                registerTerminalResultAction.TerminalCommand.TerminalCommandKey, 
                registerTerminalResultAction.TerminalCommand);

        return inTerminalResultState with
        {
            TerminalCommandResultMap = nextResultMap
        };
    }
}