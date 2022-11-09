using BlazorStudio.ClassLib.CommonComponents;
using BlazorStudio.ClassLib.Store.NuGetPackageManagerCase;
using BlazorStudio.ClassLib.Store.SolutionExplorer;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.CodeAnalysis;

namespace BlazorStudio.RazorLib.NuGet;

public partial class NuGetPackageManager : FluxorComponent, INuGetPackageManagerRendererType
{
    [Inject]
    private IState<NuGetPackageManagerState> NuGetPackageManagerStateWrap { get; set; } = null!;
    [Inject]
    private IState<SolutionExplorerState> SolutionExplorerStateWrap { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;
    
    private void SelectedProjectToModifyChanged(
        ChangeEventArgs changeEventArgs,
        SolutionExplorerState solutionExplorerState)
    {
        if (changeEventArgs.Value is null ||
            solutionExplorerState.Solution is null)
        {
            return;
        }
        
        var projectGuid = Guid.Parse((string)changeEventArgs.Value);
        
        var projectId = ProjectId.CreateFromSerialized(projectGuid);

        var selectedProject = solutionExplorerState.Solution.GetProject(projectId);
        
        Dispatcher.Dispatch(
            new NuGetPackageManagerState.SetSelectedProjectToModifyAction(
                selectedProject));       
    }

    private bool CheckIsSelected(Project project, NuGetPackageManagerState nuGetPackageManagerState)
    {
        if (nuGetPackageManagerState.SelectedProjectToModify is null)
            return false;
        
        return nuGetPackageManagerState.SelectedProjectToModify.Id.Id.ToString() == 
            project.Id.Id.ToString();
    }
}