using Fluxor;
using Microsoft.CodeAnalysis;

namespace BlazorStudio.ClassLib.Store.NuGetPackageManagerCase;

[FeatureState]
public record NuGetPackageManagerState(Project? SelectedProjectToModify)
{
    public NuGetPackageManagerState() : this(default(Project?))
    {
        
    }

    public record SetSelectedProjectToModifyAction(Project? SelectedProjectToModify);
    
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
    }
}
