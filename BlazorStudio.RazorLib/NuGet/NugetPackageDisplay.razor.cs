using BlazorStudio.ClassLib.Nuget;
using BlazorStudio.ClassLib.Store.NuGetPackageManagerCase;
using BlazorStudio.ClassLib.Store.SolutionExplorer;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.NuGet;

public partial class NugetPackageDisplay : FluxorComponent
{
    [Inject]
    private IState<NuGetPackageManagerState> NuGetPackageManagerStateWrap { get; set; } = null!;
    [Inject]
    private IState<SolutionExplorerState> SolutionExplorerStateWrap { get; set; } = null!;
    
    [Parameter, EditorRequired]
    public NugetPackageRecord NugetPackageRecord { get; set; } = null!;

    private string _nugetPackageVersionString;

    protected override void OnInitialized()
    {
        _nugetPackageVersionString = NugetPackageRecord.Version;
        
        base.OnInitialized();
    }

    private Task AddNugetPackageReferenceOnClick()
    {
        throw new NotImplementedException();
    }

    private void SelectedNugetVersionChanged(ChangeEventArgs changeEventArgs)
    {
        _nugetPackageVersionString = changeEventArgs.Value?.ToString() ?? string.Empty;
    }
    
    private bool ValidateSolutionContainsSelectedProject()
    {
        var solutionExplorerState = SolutionExplorerStateWrap.Value;
        var nuGetPackageManagerState = NuGetPackageManagerStateWrap.Value;
        
        if (solutionExplorerState.Solution is null ||
            nuGetPackageManagerState.SelectedProjectToModify is null)
            return false;
        
        return solutionExplorerState.Solution.ContainsProject(
            nuGetPackageManagerState.SelectedProjectToModify.Id);
    }
}