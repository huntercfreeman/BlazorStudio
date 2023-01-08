using BlazorALaCarte.Shared.Dimensions;
using BlazorALaCarte.Shared.Resize;
using BlazorStudio.ClassLib.Dimensions;
using BlazorStudio.ClassLib.Store.PanelCase;
using Fluxor;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.Shared;

public partial class BlazorTextEditorBody : ComponentBase
{
    [Inject]
    private IState<PanelsCollection> PanelsCollectionWrap { get; set; } = null!;
    
    [Parameter, EditorRequired]
    public ElementDimensions BodyElementDimensions { get; set; } = null!;
    
    private ElementDimensions _editorElementDimensions = new();

    protected override void OnInitialized()
    {
        var editorWidth = _editorElementDimensions.DimensionAttributes
            .Single(da => da.DimensionAttributeKind == DimensionAttributeKind.Width);
        
        editorWidth.DimensionUnits.AddRange(new []
        {
            new DimensionUnit
            {
                Value = 70,
                DimensionUnitKind = DimensionUnitKind.Percentage
            },
            new DimensionUnit
            {
                Value = ResizableColumn.RESIZE_HANDLE_WIDTH_IN_PIXELS / 2,
                DimensionUnitKind = DimensionUnitKind.Pixels,
                DimensionOperatorKind = DimensionOperatorKind.Subtract
            }
        });
        
        base.OnInitialized();
    }

    private async Task ReRenderAsync()
    {
        await InvokeAsync(StateHasChanged);
    }
}