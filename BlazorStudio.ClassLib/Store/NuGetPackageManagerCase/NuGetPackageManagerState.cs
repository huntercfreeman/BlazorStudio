using Fluxor;
using Microsoft.CodeAnalysis;

namespace BlazorStudio.ClassLib.Store.NuGetPackageManagerCase;

[FeatureState]
public record NuGetPackageManagerState(
    Project? SelectedProjectToModify, 
    string NugetQuery,
    bool IncludePrerelease)
{
    public NuGetPackageManagerState() 
        : this(default(Project?), string.Empty, true)
    {
        
    }

    public record SetSelectedProjectToModifyAction(Project? SelectedProjectToModify);
    public record SetNugetQueryAction(string NugetQuery);
    public record SetIncludePrereleaseAction(bool IncludePrerelease);
    
    private class NuGetPackageManagerStateReducer
    {
        [ReducerMethod]
        public static NuGetPackageManagerState ReduceSetSelectedProjectToModifyAction(
            NuGetPackageManagerState inNuGetPackageManagerState,
            SetSelectedProjectToModifyAction setSelectedProjectToModifyAction)
        {
            return inNuGetPackageManagerState with
            {
                SelectedProjectToModify = 
                    setSelectedProjectToModifyAction.SelectedProjectToModify
            };
        }
        
        [ReducerMethod]
        public static NuGetPackageManagerState ReduceSetNugetQueryAction(
            NuGetPackageManagerState inNuGetPackageManagerState,
            SetNugetQueryAction setNugetQueryAction)
        {
            return inNuGetPackageManagerState with
            {
                NugetQuery = 
                    setNugetQueryAction.NugetQuery
            };
        }
        
        [ReducerMethod]
        public static NuGetPackageManagerState ReduceSetIncludePrereleaseAction(
            NuGetPackageManagerState inNuGetPackageManagerState,
            SetIncludePrereleaseAction setIncludePrereleaseAction)
        {
            return inNuGetPackageManagerState with
            {
                IncludePrerelease = 
                    setIncludePrereleaseAction.IncludePrerelease
            };
        }
    }
}
