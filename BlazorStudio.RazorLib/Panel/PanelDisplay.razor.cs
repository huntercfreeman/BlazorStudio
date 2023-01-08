using BlazorStudio.ClassLib.Panel;
using BlazorStudio.ClassLib.Store.PanelCase;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorStudio.RazorLib.Panel;

public partial class PanelDisplay : FluxorComponent
{
    [Inject]
    private IState<PanelsCollection> PanelsCollectionWrap { get; set; } = null!;
    
    [Parameter, EditorRequired]
    public PanelRecordKey PanelRecordKey { get; set; } = null!;
    [Parameter, EditorRequired]
    public string CssClassString { get; set; } = null!;

    public string PanelPositionCssClass => GetPanelPositionCssClass();

    private string GetPanelPositionCssClass()
    {
        var position = string.Empty;
        
        if (PanelFacts.LeftPanelRecordKey == PanelRecordKey)
        {
            position = "left";
        }
        else if (PanelFacts.RightPanelRecordKey == PanelRecordKey)
        {
            position = "right";
        }
        else if (PanelFacts.BottomPanelRecordKey == PanelRecordKey)
        {
            position = "bottom";
        }

        return $"bstudio_panel_{position}";
    }
}
