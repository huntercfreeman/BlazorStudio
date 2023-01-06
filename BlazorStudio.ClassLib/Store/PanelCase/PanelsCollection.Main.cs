using System.Collections.Immutable;
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
            ImmutableArray<PanelTab>.Empty);
        
        var rightPanel = new PanelRecord(
            PanelFacts.RightPanelRecordKey,
            PanelTabKey.Empty,
            ImmutableArray<PanelTab>.Empty);
        
        var bottomPanel = new PanelRecord(
            PanelFacts.BottomPanelRecordKey,
            PanelTabKey.Empty,
            ImmutableArray<PanelTab>.Empty);
        
        var initialPanels = new PanelRecord[]
        {
            leftPanel,
            rightPanel,
            bottomPanel,
        }.ToImmutableArray();
        
        PanelRecordsList = initialPanels;
    }

    public ImmutableArray<PanelRecord> PanelRecordsList { get; init; }
}