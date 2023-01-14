using BlazorStudio.ClassLib.FileSystem.Classes;
using BlazorStudio.ClassLib.Git;
using Fluxor;

namespace BlazorStudio.ClassLib.Store.GitCase;

public partial record GitState
{
    private class Reducer
    {
        [ReducerMethod]
        public static GitState ReduceSetGitStateWithAction(
            GitState inGitState,
            SetGitStateWithAction setGitStateWithAction)
        {
            return setGitStateWithAction.GitStateWithFunc
                .Invoke(inGitState);
        }
    }
}

