using System.Collections.Immutable;
using BlazorStudio.ClassLib.CommandLine;
using BlazorStudio.ClassLib.CommonComponents;
using BlazorStudio.ClassLib.FileConstants;
using BlazorStudio.ClassLib.FileSystem.Classes;
using BlazorStudio.ClassLib.FileSystem.Interfaces;
using BlazorStudio.ClassLib.Nuget;
using BlazorStudio.ClassLib.Store.DialogCase;
using BlazorStudio.ClassLib.Store.NotificationCase;
using BlazorStudio.ClassLib.Store.NuGetPackageManagerCase;
using BlazorStudio.ClassLib.Store.SolutionExplorer;
using BlazorStudio.ClassLib.Store.TerminalCase;
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
    [Inject]
    private IState<TerminalSessionsState> TerminalSessionsStateWrap { get; set; } = null!;
    [Inject]
    private ICommonComponentRenderers CommonComponentRenderers { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;
    
    [Parameter, EditorRequired]
    public NugetPackageRecord NugetPackageRecord { get; set; } = null!;

    private string _nugetPackageVersionString;

    private static TerminalCommandKey _addNugetPackageTerminalCommandKey = TerminalCommandKey.NewTerminalCommandKey();

    private ImmutableArray<NugetPackageVersionRecord> _nugetPackageVersionsOrdered = ImmutableArray<NugetPackageVersionRecord>.Empty;

    private string? _previousNugetPackageId;
    
    protected override void OnParametersSet()
    {
        if (_previousNugetPackageId is null ||
            _previousNugetPackageId != NugetPackageRecord.Id)
        {
            _previousNugetPackageId = NugetPackageRecord.Id;
            
            _nugetPackageVersionsOrdered = NugetPackageRecord.Versions
                .OrderByDescending(x => x.Version)
                .ToImmutableArray();

            _nugetPackageVersionString = _nugetPackageVersionsOrdered
                .FirstOrDefault()?
                .Version
                    ?? string.Empty;
        }
        
        base.OnParametersSet();
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
    
    private async Task AddNugetPackageReferenceOnClick()
    {
        var targetProject = NuGetPackageManagerStateWrap.Value.SelectedProjectToModify;
        var targetNugetPackage = NugetPackageRecord;
        var targetNugetVersion = _nugetPackageVersionString;

        if (!ValidateSolutionContainsSelectedProject() ||
            targetProject is null ||
            targetProject.FilePath is null)
        {
            return;
        }

        var projectAbsoluteFilePath = new AbsoluteFilePath(targetProject.FilePath, false);
        
        var parentDirectory = (IAbsoluteFilePath) projectAbsoluteFilePath.Directories
            .Last();

        var interpolatedCommand = DotNetCliFacts.FormatAddNugetPackageReferenceToProject(
            targetProject.FilePath,
            targetNugetPackage.Id,
            targetNugetVersion);
        
        var addNugetPackageReferenceCommand = new TerminalCommand(
            _addNugetPackageTerminalCommandKey,
            interpolatedCommand,
            parentDirectory.GetAbsoluteFilePathString(),
            CancellationToken.None,
            async () =>
            {
                var notificationInformative  = new NotificationRecord(
                    NotificationKey.NewNotificationKey(), 
                    "Add Nuget Package Reference",
                    CommonComponentRenderers.InformativeNotificationRendererType,
                    new Dictionary<string, object?>
                    {
                        {
                            nameof(IInformativeNotificationRendererType.Message), 
                            $"{targetNugetPackage.Title}, {targetNugetVersion} was added to {targetProject}"
                        },
                    });
        
                Dispatcher.Dispatch(
                    new NotificationState.RegisterNotificationAction(
                        notificationInformative));
            });
        
        var generalTerminalSession = TerminalSessionsStateWrap.Value.TerminalSessionMap[
            TerminalSessionFacts.GENERAL_TERMINAL_SESSION_KEY];
        
        await generalTerminalSession
            .EnqueueCommandAsync(addNugetPackageReferenceCommand);
    }
}