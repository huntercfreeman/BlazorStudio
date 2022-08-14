using BlazorStudio.ClassLib.UserInterface;
using Microsoft.AspNetCore.Components;
using Microsoft.CodeAnalysis;

namespace BlazorStudio.RazorLib.SyntaxRootRender;

public partial class SyntaxNodeTreeView : ComponentBase
{
    [Parameter, EditorRequired]
    public SyntaxNode SyntaxNode { get; set; } = null!;
    
    private int _preFontSize = 15;
    
    private Dimensions _toFullStringDimensions = new Dimensions
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
    
    private Dimensions _anotherDimensions = new Dimensions
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