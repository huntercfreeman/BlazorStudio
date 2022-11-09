using BlazorStudio.ClassLib.CommonComponents;
using BlazorStudio.ClassLib.Store.NuGetPackageManagerCase;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.NuGet;

public partial class NuGetPackageManager : FluxorComponent, INuGetPackageManagerRendererType
{
    [Inject]
    private IState<NuGetPackageManagerState> NuGetPackageManagerStateWrap { get; set; } = null!;
}