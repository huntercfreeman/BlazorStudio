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

        Project? selectedProject = null;
        
        if (projectGuid != Guid.Empty)
        {
            var projectId = ProjectId.CreateFromSerialized(projectGuid);

            selectedProject = solutionExplorerState.Solution.GetProject(projectId);
        }
        
        Dispatcher.Dispatch(
            new NuGetPackageManagerState.SetSelectedProjectToModifyAction(
                selectedProject));       
    }

    private bool CheckIfProjectIsSelected(Project project, NuGetPackageManagerState nuGetPackageManagerState)
    {
        if (nuGetPackageManagerState.SelectedProjectToModify is null)
            return false;
        
        return nuGetPackageManagerState.SelectedProjectToModify.Id.Id.ToString() == 
            project.Id.Id.ToString();
    }
    
    private bool ValidateSolutionContainsSelectedProject(
        SolutionExplorerState solutionExplorerState, 
        NuGetPackageManagerState nuGetPackageManagerState)
    {
        if (solutionExplorerState.Solution is null ||
            nuGetPackageManagerState.SelectedProjectToModify is null)
            return false;
        
        return solutionExplorerState.Solution.ContainsProject(
            nuGetPackageManagerState.SelectedProjectToModify.Id);
    }
}