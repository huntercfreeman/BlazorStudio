using BlazorStudio.ClassLib.UserInterface;

namespace BlazorStudio.ClassLib.Store.DialogCase;

public record DialogRecord(DialogKey DialogKey,
    string Title,
    Type Type,
    Dictionary<string, object?>? Parameters)
{
    public Dimensions Dimensions { get; init; } = ConstructDefaultDialogDimensions();
    public bool IsMinimized { get; init; }
    public bool IsMaximized { get; set; }
    public bool IsTransformable { get; set; } = true;
    public event EventHandler? OnFocusRequestedEventHandler;

    public void InvokeOnFocusRequestedEventHandler()
    {
        OnFocusRequestedEventHandler?.Invoke(null, EventArgs.Empty);
    }
    
    public static Dimensions ConstructDefaultDialogDimensions()
    {
        return new Dimensions
        {
            DimensionsPositionKind = DimensionsPositionKind.Fixed,
            WidthCalc = new List<DimensionUnit>
            {
                new()
                {
                    DimensionUnitKind = DimensionUnitKind.ViewportWidth,
                    Value = 60
                }
            },
            HeightCalc = new List<DimensionUnit>
            {
                new()
                {
                    DimensionUnitKind = DimensionUnitKind.ViewportHeight,
                    Value = 60
                }
            },
            LeftCalc = new List<DimensionUnit>
            {
                new()
                {
                    DimensionUnitKind = DimensionUnitKind.ViewportWidth,
                    Value = 20
                }
            },
            TopCalc = new List<DimensionUnit>
            {
                new()
                {
                    DimensionUnitKind = DimensionUnitKind.ViewportHeight,
                    Value = 20
                }
            },
        };
    }
}