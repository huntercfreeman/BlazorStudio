using BlazorStudio.ClassLib.Family;
using BlazorStudio.ClassLib.Store.ThemeCase;
using BlazorStudio.ClassLib.UserInterface;
using BlazorStudio.ClassLib.Virtualize;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.Shared;

public partial class MainLayout : FluxorLayout
{
    [Inject]
    private IState<ThemeState> ThemeStateWrap { get; set; } = null!;

    private List<Person> _people = new List<Person>();
    private VirtualizeCoordinateSystemResult<Row> _initialVirtualizeCoordinateSystemResult;
    private DimensionUnit _characterFontSize = new DimensionUnit()
    {
        DimensionUnitKind = DimensionUnitKind.RootCharacterHeight,
        Value = 1
    };

    private List<Row> _rows = new List<Row>();

    private int rowCount = 200;
    private int characterCount = 200;

    private bool _printSpace;
    private bool _printEmptyRow;

    private class Row
    {
        public List<Character> RowText { get; set; } = new();
    }
    
    private class Character
    {
        /// <summary>
        /// Some escaped Html Characters like &nbsp; require more than
        /// 1 character to represent itself therefore this is a string Type
        /// </summary>
        public string Value { get; set; }
    }

    protected override void OnInitialized()
    {
        for (int i = 0; i < rowCount; i++)
        {
            var row = new Row();

            _printEmptyRow = !_printEmptyRow;

            if (_printEmptyRow)
            {
                row.RowText.Add(new Character() { Value = _printSpace ? "    " : "." });
            }
            else
            {
                for (int j = 0; j < characterCount; j++)
                {
                    _printSpace = !_printSpace;

                    row.RowText.Add(new Character() { Value = _printSpace ? "    " : "." });
                }
            }

            _rows.Add(row);
        }

        var dimensionsOfCoordinateSystem = new Dimensions()
        {
            WidthCalc = new List<DimensionUnit>()
            {
                new DimensionUnit()
                {
                    DimensionUnitKind = DimensionUnitKind.RootCharacterHeight,
                    Value = 50
                }
            },
            HeightCalc = new List<DimensionUnit>()
            {
                new DimensionUnit()
                {
                    DimensionUnitKind = DimensionUnitKind.RootCharacterHeight,
                    Value = 50
                }
            },
            LeftCalc = new List<DimensionUnit>()
            {
                new DimensionUnit()
                {
                    DimensionUnitKind = DimensionUnitKind.Percentage,
                    Value = 25
                }
            },
            TopCalc = new List<DimensionUnit>()
            {
                new DimensionUnit()
                {
                    DimensionUnitKind = DimensionUnitKind.Percentage,
                    Value = 8
                }
            },
        };

        var dimensionsOfLeftBoundary = new Dimensions()
        {
            WidthCalc = new List<DimensionUnit>()
            {
                new DimensionUnit()
                {
                    DimensionUnitKind = DimensionUnitKind.RootCharacterHeight,
                    Value = 2
                }
            },
            HeightCalc = new List<DimensionUnit>()
            {
                new DimensionUnit()
                {
                    DimensionUnitKind = DimensionUnitKind.Percentage,
                    Value = 100
                }
            }
        };

        var dimensionsOfRightBoundary = new Dimensions()
        {
            WidthCalc = new List<DimensionUnit>()
            {
                new DimensionUnit()
                {
                    DimensionUnitKind = DimensionUnitKind.RootCharacterHeight,
                    Value = 2
                }
            },
            HeightCalc = new List<DimensionUnit>()
            {
                new DimensionUnit()
                {
                    DimensionUnitKind = DimensionUnitKind.Percentage,
                    Value = 100
                }
            },
            LeftCalc = new List<DimensionUnit>()
            {
                new DimensionUnit()
                {
                    DimensionUnitKind = DimensionUnitKind.Pixels,
                    Value = 100
                }
            },
        };

        var dimensionsOfTopBoundary = new Dimensions()
        {
            WidthCalc = new List<DimensionUnit>()
            {
                new DimensionUnit()
                {
                    DimensionUnitKind = DimensionUnitKind.Percentage,
                    Value = 100
                }
            },
            HeightCalc = new List<DimensionUnit>()
            {
                new DimensionUnit()
                {
                    DimensionUnitKind = DimensionUnitKind.RootCharacterHeight,
                    Value = 2
                }
            }
        };

        var dimensionsOfBottomBoundary = new Dimensions()
        {
            WidthCalc = new List<DimensionUnit>()
            {
                new DimensionUnit()
                {
                    DimensionUnitKind = DimensionUnitKind.Percentage,
                    Value = 100
                }
            },
            HeightCalc = new List<DimensionUnit>()
            {
                new DimensionUnit()
                {
                    DimensionUnitKind = DimensionUnitKind.RootCharacterHeight,
                    Value = 2
                }
            },
            BottomCalc = new List<DimensionUnit>()
            {
                new DimensionUnit()
                {
                    DimensionUnitKind = DimensionUnitKind.Pixels,
                    Value = 100
                }
            }
        };

        _initialVirtualizeCoordinateSystemResult = new(_rows, 
            dimensionsOfCoordinateSystem,
            dimensionsOfLeftBoundary,
            dimensionsOfRightBoundary,
            dimensionsOfTopBoundary,
            dimensionsOfBottomBoundary);

        base.OnInitialized();
    }
}