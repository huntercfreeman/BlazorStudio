using Fluxor;

namespace BlazorStudio.ClassLib.Store.TerminalCase;

public partial record TerminalSession
{
    private class TerminalSessionsStateReducer
    {
        [ReducerMethod]
        public static TerminalSessionsState ReduceSetWorkingDirectoryAbsoluteFilePathStringAction(
            TerminalSessionsState inTerminalSessionsState,
            TerminalSessionsState.SetWorkingDirectoryAbsoluteFilePathStringAction setWorkingDirectoryAbsoluteFilePathStringAction)
        {
            if (inTerminalSessionsState.TerminalSessionMap.TryGetValue(
                    setWorkingDirectoryAbsoluteFilePathStringAction.TerminalSessionKey,
                    out var terminalSession))
            {
                terminalSession = terminalSession with
                {
                    WorkingDirectoryAbsoluteFilePathString = setWorkingDirectoryAbsoluteFilePathStringAction
                        .WorkingDirectoryAbsoluteFilePathString
                };
                
                var nextTerminalSessionMap = inTerminalSessionsState.TerminalSessionMap
                    .SetItem(terminalSession.TerminalSessionKey, terminalSession);

                return inTerminalSessionsState with
                {
                    TerminalSessionMap = nextTerminalSessionMap
                };
            }

            return inTerminalSessionsState;
        }
    }
}