using BlazorStudio.ClassLib.Sequence;
using Fluxor;

namespace BlazorStudio.ClassLib.Store.NugetPackageManagerCase;

public class NugetPackageManagerStateReducer
{
    [ReducerMethod(typeof(RequestFocusOnNugetPackageManagerAction))]
    public static NugetPackageManagerState ReduceRequestFocusOnNugetPackageManagerAction(
        NugetPackageManagerState previousNugetPackageManagerState)
    {
        return previousNugetPackageManagerState with
        {
            FocusRequestedSequenceKey = SequenceKey.NewSequenceKey(),
        };
    }
}