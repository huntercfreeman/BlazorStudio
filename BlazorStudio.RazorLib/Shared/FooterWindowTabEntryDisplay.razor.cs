using BlazorStudio.ClassLib.Store.FooterWindowCase;
using Fluxor;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.Shared;

public partial class FooterWindowTabEntryDisplay : ComponentBase
{
    [Inject]
    private IState<FooterWindowState> FooterWindowStateWrap { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;
    
    [Parameter, EditorRequired]
    public FooterWindowTabEntry FooterWindowTabEntry { get; set; } = null!;
    [Parameter, EditorRequired]
    public int FooterWindowTabIndex { get; set; }

    private string IsActiveCssClassString => 
        FooterWindowStateWrap.Value.ActiveFooterWindowKind == FooterWindowTabEntry.FooterWindowKind
            ? "bstudio_active"
            : string.Empty;

    private void DispatchSetActiveFooterWindowKindAction()
    {
        Dispatcher
            .Dispatch(new SetActiveFooterWindowKindAction(FooterWindowTabEntry.FooterWindowKind));       
    }
}