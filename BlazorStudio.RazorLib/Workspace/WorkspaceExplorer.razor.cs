using BlazorStudio.ClassLib.Store.WorkspaceCase;
using BlazorStudio.ClassLib.UserInterface;
using Fluxor;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.Workspace;

public partial class WorkspaceExplorer : ComponentBase
{
    [Inject]
    private IState<WorkspaceState> WorkspaceStateWrap { get; set; } = null!;

    [Parameter, EditorRequired]
    public Dimensions Dimensions { get; set; } = null!;
}