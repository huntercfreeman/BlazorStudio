using BlazorStudio.ClassLib.Store.SolutionCase;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.FileDocumentMap;

public partial class FileDocumentMapDisplay : FluxorComponent
{
    [Inject]
    private IState<SolutionState> SolutionStateWrap { get; set; } = null!;
}