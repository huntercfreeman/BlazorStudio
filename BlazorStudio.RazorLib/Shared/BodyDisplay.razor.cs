using BlazorStudio.ClassLib.UserInterface;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.Shared;

public partial class BodyDisplay : ComponentBase
{
    private Dimensions _editingSectionDimensions = new()
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
                Value = 75,
            },
            new()
            {
                DimensionUnitKind = DimensionUnitKind.Pixels,
                Value = -2.5,
            },
        },
    };

    private Dimensions _editorDisplayDimensions = new()
    {
        DimensionsPositionKind = DimensionsPositionKind.Static,
        WidthCalc = new List<DimensionUnit>
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
        HeightCalc = new List<DimensionUnit>
        {
            new()
            {
                DimensionUnitKind = DimensionUnitKind.Percentage,
                Value = 100,
            },
        },
    };

    private Dimensions _explorersDimensions = new()
    {
        DimensionsPositionKind = DimensionsPositionKind.Static,
        WidthCalc = new List<DimensionUnit>
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
        HeightCalc = new List<DimensionUnit>
        {
            new()
            {
                DimensionUnitKind = DimensionUnitKind.Percentage,
                Value = 100,
            },
        },
    };

    private Dimensions _footerWindowDimensions = new()
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
                Value = 25,
            },
            new()
            {
                DimensionUnitKind = DimensionUnitKind.Pixels,
                Value = -5,
            },
        },
    };

    private async Task ReRender()
    {
        await InvokeAsync(StateHasChanged);
    }
}