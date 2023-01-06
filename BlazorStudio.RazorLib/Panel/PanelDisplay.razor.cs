using BlazorStudio.ClassLib.Panel;
using BlazorStudio.ClassLib.Store.PanelCase;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.Panel;

public partial class PanelDisplay : FluxorComponent
{
    [Inject]
    private IState<PanelsCollection> PanelsCollectionWrap { get; set; } = null!;
    
    [Parameter, EditorRequired]
    public PanelRecordKey PanelRecordKey { get; set; } = null!;

    public string PanelPositionCssClass => GetPanelPositionCssClass();
    
    public string GetPanelPositionCssClass()
    {
        var position = string.Empty;
        
        if (PanelFacts.LeftPanelRecordKey == PanelRecordKey)
        {
            return "left";
        }
        else if (PanelFacts.RightPanelRecordKey == PanelRecordKey)
        {
            return "right";
        }
        else if (PanelFacts.BottomPanelRecordKey == PanelRecordKey)
        {
            return "bottom";
        }

        return $"bstudio_panel_{position}";
    }
}
