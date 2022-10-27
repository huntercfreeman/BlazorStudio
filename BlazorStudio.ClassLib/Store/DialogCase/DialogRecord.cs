using BlazorStudio.ClassLib.Dimensions;

namespace BlazorStudio.ClassLib.Store.DialogCase;

public record DialogRecord(
    DialogKey DialogKey,
    string Title,
    Type RendererType,
    Dictionary<string, object?>? Parameters)
{
    public ElementDimensions ElementDimensions { get; init; } = ConstructDefaultDialogDimensions();
    public bool IsMinimized { get; set; }
    public bool IsMaximized { get; set; }
    public bool IsResizable { get; set; } = true;

    public static ElementDimensions ConstructDefaultDialogDimensions()
    {
        var elementDimensions = new ElementDimensions
        {
            ElementPositionKind = ElementPositionKind.Fixed
        };
        
        // Width
        {
            var width = elementDimensions.DimensionAttributes
                .Single(x => x.DimensionAttributeKind == DimensionAttributeKind.Width);

            width.DimensionUnits.Add(new DimensionUnit
            {
                Value = 60,
                DimensionUnitKind = DimensionUnitKind.ViewportWidth
            });
        }
        
        // Height
        {
            var height = elementDimensions.DimensionAttributes
                .Single(x => x.DimensionAttributeKind == DimensionAttributeKind.Height);

            height.DimensionUnits.Add(new DimensionUnit
            {
                Value = 60,
                DimensionUnitKind = DimensionUnitKind.ViewportHeight
            });
        }
        
        // Left
        {
            var left = elementDimensions.DimensionAttributes
                .Single(x => x.DimensionAttributeKind == DimensionAttributeKind.Left);

            left.DimensionUnits.Add(new DimensionUnit
            {
                Value = 20,
                DimensionUnitKind = DimensionUnitKind.ViewportWidth
            });
        }
        
        // Top
        {
            var top = elementDimensions.DimensionAttributes
                .Single(x => x.DimensionAttributeKind == DimensionAttributeKind.Top);

            top.DimensionUnits.Add(new DimensionUnit
            {
                Value = 20,
                DimensionUnitKind = DimensionUnitKind.ViewportHeight
            });
        }

        return elementDimensions;
    }
}