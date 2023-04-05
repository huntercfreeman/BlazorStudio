using System.Collections.Immutable;
using BlazorStudio.ClassLib.ComponentRenderers;
using BlazorStudio.ClassLib.Nuget;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.NuGet;

public partial class NuGetPackageManager : FluxorComponent, INuGetPackageManagerRendererType
{
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;
    [Inject]
    private INugetPackageManagerProvider NugetPackageManagerProvider { get; set; } = null!;

    private bool _performingNugetQuery;

    private Exception? _exceptionFromNugetQuery;
}