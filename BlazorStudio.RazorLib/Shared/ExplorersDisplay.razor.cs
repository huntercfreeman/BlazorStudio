using BlazorStudio.ClassLib.UserInterface;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.Shared;

public partial class ExplorersDisplay : ComponentBase
{
    [Parameter, EditorRequired]
    public Dimensions ExplorersDimensions { get; set; } = null!;

    private Dimensions _solutionExplorerDimensions = new Dimensions
    {
        DimensionsPositionKind = DimensionsPositionKind.Static,
        WidthCalc = new List<DimensionUnit>
        {
            new()
            {
                DimensionUnitKind = DimensionUnitKind.Percentage,
                Value = 100
            }
        },
        HeightCalc = new List<DimensionUnit>
        {
            new()
            {
                DimensionUnitKind = DimensionUnitKind.Percentage,
                Value = 50
            },
            new()
            {
                DimensionUnitKind = DimensionUnitKind.Pixels,
                Value = -4
            },
        }
    };

    private Dimensions _workspaceExplorerDimensions = new Dimensions
    {
        DimensionsPositionKind = DimensionsPositionKind.Static,
        WidthCalc = new List<DimensionUnit>
        {
            new()
            {
                DimensionUnitKind = DimensionUnitKind.Percentage,
                Value = 100
            }
        },
        HeightCalc = new List<DimensionUnit>
        {
            new()
            {
                DimensionUnitKind = DimensionUnitKind.Percentage,
                Value = 50
            },
            new()
            {
                DimensionUnitKind = DimensionUnitKind.Pixels,
                Value = -4
            },
        }
    };

    private async Task ReRender()
    {
        await InvokeAsync(StateHasChanged);
    }
}