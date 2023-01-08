using BlazorStudio.ClassLib.Panel;
using BlazorStudio.ClassLib.Store.PanelCase;
using Fluxor;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.Panel;

public partial class PanelTabDisplay : ComponentBase
{
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;
    
    [Parameter, EditorRequired]
    public PanelTab PanelTab { get; set; } = null!;
    [Parameter, EditorRequired]
    public PanelRecord PanelRecord { get; set; } = null!;

    private string IsActiveCssClassString => PanelRecord.ActivePanelTabKey == PanelTab.PanelTabKey
        ? "balc_active"
        : string.Empty;

    private void DispatchSetActivePanelTabActionOnClick()
    {
        Dispatcher.Dispatch(new PanelsCollection.SetActivePanelTabAction(
            PanelRecord.PanelRecordKey,
            PanelTab.PanelTabKey));
    }
}