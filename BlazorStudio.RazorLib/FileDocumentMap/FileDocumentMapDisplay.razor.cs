using BlazorStudio.ClassLib.Store.SolutionCase;
using BlazorStudio.ClassLib.Store.TreeViewCase;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.CodeAnalysis;

namespace BlazorStudio.RazorLib.FileDocumentMap;

public partial class FileDocumentMapDisplay : FluxorComponent
{
    [Inject]
    private IState<SolutionState> SolutionStateWrap { get; set; } = null!;
}