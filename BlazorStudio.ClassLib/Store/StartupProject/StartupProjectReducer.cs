using Fluxor;

namespace BlazorStudio.ClassLib.Store.StartupProject;

public class StartupProjectReducer
{
    [ReducerMethod]
    public static StartupProjectState ReduceSetStartupProjectAction(StartupProjectState previousStartupProjectState,
        SetStartupProjectAction setStartupProjectAction)
    {
        return new StartupProjectState(setStartupProjectAction.ProjectAbsoluteFilePath);
    }
}