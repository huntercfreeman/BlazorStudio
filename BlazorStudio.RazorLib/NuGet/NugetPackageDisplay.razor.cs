using System.Collections.Immutable;
using BlazorCommon.RazorLib.Notification;
using BlazorCommon.RazorLib.Store.NotificationCase;
using BlazorStudio.ClassLib.CommandLine;
using BlazorStudio.ClassLib.CommonComponents;
using BlazorStudio.ClassLib.FileConstants;
using BlazorStudio.ClassLib.FileSystem.Classes;
using BlazorStudio.ClassLib.FileSystem.Classes.FilePath;
using BlazorStudio.ClassLib.FileSystem.Interfaces;
using BlazorStudio.ClassLib.Nuget;
using BlazorStudio.ClassLib.Store.TerminalCase;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.NuGet;

public partial class NugetPackageDisplay : FluxorComponent
{
    [Inject]
    private IState<TerminalSessionsState> TerminalSessionsStateWrap { get; set; } = null!;
    [Inject]
    private ICommonComponentRenderers CommonComponentRenderers { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;
    [Inject]
    private IEnvironmentProvider EnvironmentProvider { get; set; } = null!;
    
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
        return false;
    }
    
    private async Task AddNugetPackageReferenceOnClick()
    {
        return;
    }
}