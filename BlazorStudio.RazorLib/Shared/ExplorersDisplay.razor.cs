using BlazorStudio.ClassLib.UserInterface;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.Shared;

public partial class ExplorersDisplay : ComponentBase
{
    private Dimensions _folderExplorerDimensions = new()
    {
        DimensionsPositionKind = DimensionsPositionKind.Static,
        WidthCalc = new List<DimensionUnit>
        {
            new()
            {
                DimensionUnitKind = DimensionUnitKind.Percentage,
                Value = 100,
            },
        },
        HeightCalc = new List<DimensionUnit>
        {
            new()
            {
                DimensionUnitKind = DimensionUnitKind.Percentage,
                Value = 50,
            },
            new()
            {
                DimensionUnitKind = DimensionUnitKind.Pixels,
                Value = -2.5,
            },
        },
    };

    private Dimensions _solutionExplorerDimensions = new()
    {
        DimensionsPositionKind = DimensionsPositionKind.Static,
        WidthCalc = new List<DimensionUnit>
        {
            new()
            {
                DimensionUnitKind = DimensionUnitKind.Percentage,
                Value = 100,
            },
        },
        HeightCalc = new List<DimensionUnit>
        {
            new()
            {
                DimensionUnitKind = DimensionUnitKind.Percentage,
                Value = 50,
            },
            new()
            {
                DimensionUnitKind = DimensionUnitKind.Pixels,
                Value = -2.5,
            },
        },
    };

    [Parameter]
    [EditorRequired]
    public Dimensions ExplorersDimensions { get; set; } = null!;

    private async Task ReRender()
    {
        await InvokeAsync(StateHasChanged);
    }
}