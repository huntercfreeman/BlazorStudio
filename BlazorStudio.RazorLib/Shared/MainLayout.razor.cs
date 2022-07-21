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
    private VirtualizeCoordinateSystemResult<Person> _initialVirtualizeCoordinateSystemResult;
    private double _personWidth = 50;
    private double _personHeight = 50;
    private int _personInitialCount = 100;

    protected override void OnInitialized()
    {
        for (int i = 0; i < _personInitialCount; i++)
        {
            _people.Add(new Person(i.ToString(), i.ToString()));
        }

        var dimensionsOfCoordinateSystem = new Dimensions()
        {
            WidthCalc = new List<DimensionUnit>()
            {
                new DimensionUnit()
                {
                    DimensionUnitKind = DimensionUnitKind.Pixels,
                    Value = 5 * _personWidth
                }
            },
            HeightCalc = new List<DimensionUnit>()
            {
                new DimensionUnit()
                {
                    DimensionUnitKind = DimensionUnitKind.Pixels,
                    Value = 5 * _personHeight
                }
            },
        };

        _initialVirtualizeCoordinateSystemResult = new(_people, 
            dimensionsOfCoordinateSystem);

        base.OnInitialized();
    }
}