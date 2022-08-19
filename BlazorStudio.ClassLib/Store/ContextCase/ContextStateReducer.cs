using Fluxor;

namespace BlazorStudio.ClassLib.Store.ContextCase;

public class ContextStateReducer
{
    [ReducerMethod]
    public static ContextState ReduceRegisterContextStateAction(ContextState previousContextState,
        RegisterContextStateAction registerContextStateAction)
    {
        var nextImmutableList = previousContextState.ContextRecords.Add(
            registerContextStateAction.ContextRecord.ContextKey, registerContextStateAction.ContextRecord);

        return previousContextState with
        {
            ContextRecords = nextImmutableList
        };
    }
    
    [ReducerMethod]
    public static ContextState ReduceDisposeContextStateAction(ContextState previousContextState,
        DisposeContextStateAction disposeContextStateAction)
    {
        var nextImmutableList = previousContextState.ContextRecords.Remove(
            disposeContextStateAction.ContextKey);

        return previousContextState with
        {
            ContextRecords = nextImmutableList
        }; 
    }
    
    [ReducerMethod]
    public static ContextState ReduceSetActiveContextStatesAction(ContextState previousContextState,
        SetActiveContextStatesAction setActiveContextStatesAction)
    {
        return previousContextState with
        {
            ActiveContextRecords = setActiveContextStatesAction.ActiveContextStates
        }; 
    }
}