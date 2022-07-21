using BlazorStudio.ClassLib.Family;
using BlazorStudio.ClassLib.Store.DialogCase;
using BlazorStudio.ClassLib.UserInterface;
using BlazorStudio.ClassLib.Virtualize;
using BlazorStudio.RazorLib.VirtualizeComponents;
using Fluxor;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.Shared;

public partial class DebuggingVirtualizeCoordinateSystemWrapper : ComponentBase
{
    [Inject]
    private IState<DialogStates> DialogStatesWrap { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    private DialogRecord _virtualizeCoordinateSystemDialog = null!;

    private DialogRecord _debugDataVirtualizeCoordinateSystemDialog = null!;

    private void OpenVirtualizeCoordinateSystemDialogDialogOnClick()
    {
        if (DialogStatesWrap.Value.List.All(x => x.DialogKey != _virtualizeCoordinateSystemDialog.DialogKey))
            Dispatcher.Dispatch(new RegisterDialogAction(_virtualizeCoordinateSystemDialog));
    }
    
    private void OpenDebugDataVirtualizeCoordinateSystemDialogDialogOnClick()
    {
        if (DialogStatesWrap.Value.List.All(x => x.DialogKey != _debugDataVirtualizeCoordinateSystemDialog.DialogKey))
            Dispatcher.Dispatch(new RegisterDialogAction(_debugDataVirtualizeCoordinateSystemDialog));
    }

    private VirtualizeCoordinateSystemResult<DebugRow> _initialVirtualizeCoordinateSystemResult;
    private VirtualizeCoordinateSystem<DebugRow> _renderedComponent;

    private List<DebugRow> _rows = new List<DebugRow>();

    private int rowCount = 200;
    private int characterCount = 200;
    private bool _printSpace;
    private bool _printEmptyRow;

    public class DebugRow
    {
        public List<DebugCharacter> RowText { get; set; } = new();
    }

    public class DebugCharacter
    {
        /// <summary>
        /// Some escaped Html Characters like &nbsp; require more than
        /// 1 character to represent itself therefore this is a string Type
        /// </summary>
        public string Value { get; set; }
    }

    protected override void OnInitialized()
    {
        _virtualizeCoordinateSystemDialog = new DialogRecord(
            DialogKey.NewDialogKey(),
            "VirtualizeCoordinateSystem<T>",
            typeof(DebuggingVirtualizeCoordinateSystem),
            new()
            {
                {
                    nameof(DebuggingVirtualizeCoordinateSystem.GetVirtualizeCoordinateSystemResultFunc),
                    () => _initialVirtualizeCoordinateSystemResult
                },
                {
                    nameof(DebuggingVirtualizeCoordinateSystem.OnAfterRenderVirtualizeCoordinateSystemCallback),
                    new Action<VirtualizeCoordinateSystem<DebugRow>>((renderedComponent) => _renderedComponent = renderedComponent) 
                },
            }
        );

        _debugDataVirtualizeCoordinateSystemDialog = new DialogRecord(
            DialogKey.NewDialogKey(),
            "Debug Data VirtualizeCoordinateSystem<T>",
            typeof(DebugVirtualizeCoordinateSystem<DebugRow>),
            new()
            {
                {
                    nameof(DebugVirtualizeCoordinateSystem<DebugRow>.GetVirtualizeCoordinateSystemComponentFunc),
                    () => _renderedComponent
                },
            }
        );

        for (int i = 0; i < rowCount; i++)
        {
            var row = new DebugRow();

            _printEmptyRow = !_printEmptyRow;

            if (_printEmptyRow)
            {
                row.RowText.Add(new DebugCharacter() { Value = _printSpace ? "    " : "." });
            }
            else
            {
                for (int j = 0; j < characterCount; j++)
                {
                    _printSpace = !_printSpace;

                    row.RowText.Add(new DebugCharacter() { Value = _printSpace ? "    " : "." });
                }
            }

            _rows.Add(row);
        }

        var dimensionsOfCoordinateSystem = new ClassLib.UserInterface.Dimensions()
        {
            DimensionsPositionKind = DimensionsPositionKind.Relative,
            WidthCalc = new List<DimensionUnit>()
            {
                new DimensionUnit()
                {
                    DimensionUnitKind = DimensionUnitKind.RootCharacterHeight,
                    Value = 40
                }
            },
            HeightCalc = new List<DimensionUnit>()
            {
                new DimensionUnit()
                {
                    DimensionUnitKind = DimensionUnitKind.RootCharacterHeight,
                    Value = 40
                }
            },
            ArbitraryDimensionUnitLists = new List<ArbitraryDimensionUnitList>
            {
                new ArbitraryDimensionUnitList
                {
                    StyleAttributeName = "margin-top",
                    DimensionUnits = new List<DimensionUnit>()
                    {
                        new DimensionUnit()
                        {
                            DimensionUnitKind = DimensionUnitKind.Pixels,
                            Value = 40
                        }
                    }
                },
                new ArbitraryDimensionUnitList
                {
                    StyleAttributeName = "margin-left",
                    DimensionUnits =new List<DimensionUnit>()
                    {
                        new DimensionUnit()
                        {
                            DimensionUnitKind = DimensionUnitKind.Pixels,
                            Value = 40
                        }
                    }
                },
            }
        };

        var dimensionsOfLeftBoundary = new ClassLib.UserInterface.Dimensions()
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

        var dimensionsOfRightBoundary = new ClassLib.UserInterface.Dimensions()
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

        var dimensionsOfTopBoundary = new ClassLib.UserInterface.Dimensions()
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

        var dimensionsOfBottomBoundary = new ClassLib.UserInterface.Dimensions()
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