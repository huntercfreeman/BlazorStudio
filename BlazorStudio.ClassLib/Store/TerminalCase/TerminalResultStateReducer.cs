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
    
    [ReducerMethod]
    public static TerminalResultState ReduceReplaceTerminalResultAction(
        TerminalResultState inTerminalResultState,
        ReplaceTerminalResultAction replaceTerminalResultAction)
    {
        var nextResultMap = inTerminalResultState.TerminalCommandResultMap
            .SetItem(
                replaceTerminalResultAction.TerminalCommand.TerminalCommandKey, 
                replaceTerminalResultAction.TerminalCommand);

        return inTerminalResultState with
        {
            TerminalCommandResultMap = nextResultMap
        };
    }
}