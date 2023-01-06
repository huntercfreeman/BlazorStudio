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
}