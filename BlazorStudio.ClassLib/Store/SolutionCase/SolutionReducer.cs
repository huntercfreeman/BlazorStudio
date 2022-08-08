using BlazorStudio.ClassLib.FileSystem.Classes;
using Fluxor;

namespace BlazorStudio.ClassLib.Store.SolutionCase;

public class SolutionReducer
{
    [ReducerMethod]
    public static SolutionState ReduceSetSolutionWorkspaceAction(SolutionState previousSolutionState,
        SetSolutionAction setSolutionAction)
    {
        var msBuildPathString = setSolutionAction.VisualStudioInstance.MSBuildPath;

        if (msBuildPathString.EndsWith(Path.DirectorySeparatorChar) ||
            msBuildPathString.EndsWith(Path.AltDirectorySeparatorChar))
        {
            msBuildPathString = msBuildPathString.Substring(0, msBuildPathString.Length - 1);
        }

        var msBuildAbsoluteFilePath = new AbsoluteFilePath(msBuildPathString, true);

        return new(setSolutionAction.SolutionWorkspace, setSolutionAction.VisualStudioInstance, msBuildAbsoluteFilePath);
    }
}