using BlazorStudio.ClassLib.CommonComponents;
using BlazorStudio.ClassLib.Store.NuGetPackageManagerCase;
using Fluxor;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.NuGet;

public partial class NuGetPackageManager : ComponentBase, INuGetPackageManagerRendererType
{
    [Inject]
    private IState<NuGetPackageManagerState> NuGetPackageManagerStateWrap { get; set; } = null!;
}