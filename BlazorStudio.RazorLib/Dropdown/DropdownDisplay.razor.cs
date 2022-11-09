using BlazorStudio.ClassLib.Dropdown;
using BlazorStudio.ClassLib.Store.DropdownCase;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorStudio.RazorLib.Dropdown;

public partial class DropdownDisplay : FluxorComponent
{
    [Inject]
    private IState<DropdownStates> DropdownStatesWrap { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;
    
    [Parameter, EditorRequired]
    public DropdownKey DropdownKey { get; set; } = null!;
    [Parameter, EditorRequired]
    public RenderFragment ChildContent { get; set; } = null!;
    [Parameter, EditorRequired]
    public DropdownPositionKind DropdownPositionKind { get; set; } = DropdownPositionKind.Vertical;
    [Parameter]
    public bool ShouldDisplayOutOfBoundsClickDisplay { get; set; } = true;
    [Parameter]
    public string CssStyleString { get; set; } = string.Empty;
    
    private bool ShouldDisplay => DropdownStatesWrap.Value.ActiveDropdownKeys
        .Contains(DropdownKey);
    
    private string DropdownPositionKindStyleCss => DropdownPositionKind switch
    {
        DropdownPositionKind.Vertical => "position: absolute; left: 0; top: 100%;",
        DropdownPositionKind.Horizontal => "position: absolute; left: 100%; top: 0;",
        DropdownPositionKind.Unset => string.Empty,
        _ => throw new ApplicationException($"The {nameof(DropdownPositionKind)}: {DropdownPositionKind} was unrecognized.") 
    };

    private void ClearAllActiveDropdownKeys(MouseEventArgs mouseEventArgs)
    {
        Dispatcher.Dispatch(new ClearActiveDropdownKeysAction());
    }
    
    protected override void Dispose(bool disposing)
    {
        if (ShouldDisplay)
            Dispatcher.Dispatch(new RemoveActiveDropdownKeyAction(DropdownKey));
        
        base.Dispose(disposing);
    }
}


