using System.Collections.Immutable;
using BlazorALaCarte.Shared.Dimensions;
using BlazorALaCarte.Shared.Resize;
using BlazorStudio.ClassLib.Dimensions;
using BlazorStudio.ClassLib.Panel;
using Fluxor;

namespace BlazorStudio.ClassLib.Store.PanelCase;

[FeatureState]
public partial record PanelsCollection
{
    public PanelsCollection()
    {
        var leftPanel = new PanelRecord(
            PanelFacts.LeftPanelRecordKey,
            PanelTabKey.Empty,
            new ElementDimensions(),
            ImmutableArray<PanelTab>.Empty);
        
        var rightPanel = new PanelRecord(
            PanelFacts.RightPanelRecordKey,
            PanelTabKey.Empty,
            new ElementDimensions(),
            ImmutableArray<PanelTab>.Empty);
        
        var bottomPanel = new PanelRecord(
            PanelFacts.BottomPanelRecordKey,
            PanelTabKey.Empty,
            new ElementDimensions(),
            ImmutableArray<PanelTab>.Empty);
        
        var initialPanels = new PanelRecord[]
        {
            leftPanel,
            rightPanel,
            bottomPanel,
        }.ToImmutableArray();
        
        PanelRecordsList = initialPanels;
        
        var bottomPanelHeight = bottomPanel.ElementDimensions.DimensionAttributes
            .Single(da => da.DimensionAttributeKind == DimensionAttributeKind.Height);
        
        bottomPanelHeight.DimensionUnits.AddRange(new []
        {
            new DimensionUnit
            {
                Value = 22,
                DimensionUnitKind = DimensionUnitKind.Percentage
            },
            new DimensionUnit
            {
                Value = ResizableRow.RESIZE_HANDLE_HEIGHT_IN_PIXELS / 2,
                DimensionUnitKind = DimensionUnitKind.Pixels,
                DimensionOperatorKind = DimensionOperatorKind.Subtract
            },
            new DimensionUnit
            {
                Value = SizeFacts.Bstudio.Header.Height.Value / 2,
                DimensionUnitKind = SizeFacts.Bstudio.Header.Height.DimensionUnitKind,
                DimensionOperatorKind = DimensionOperatorKind.Subtract
            }
        });
        
        var leftPanelWidth = leftPanel.ElementDimensions.DimensionAttributes
            .Single(da => da.DimensionAttributeKind == DimensionAttributeKind.Width);
        
        leftPanelWidth.DimensionUnits.AddRange(new []
        {
            new DimensionUnit
            {
                Value = 30,
                DimensionUnitKind = DimensionUnitKind.Percentage
            },
            new DimensionUnit
            {
                Value = ResizableColumn.RESIZE_HANDLE_WIDTH_IN_PIXELS / 2,
                DimensionUnitKind = DimensionUnitKind.Pixels,
                DimensionOperatorKind = DimensionOperatorKind.Subtract
            }
        });

        var rightPanelWidth = rightPanel.ElementDimensions.DimensionAttributes
            .Single(da => da.DimensionAttributeKind == DimensionAttributeKind.Width);
        
        rightPanelWidth.DimensionUnits.AddRange(new []
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
    }

    public ImmutableArray<PanelRecord> PanelRecordsList { get; init; }
}