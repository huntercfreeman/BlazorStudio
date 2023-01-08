using System.Collections.Immutable;
using BlazorALaCarte.Shared.Dimensions;

namespace BlazorStudio.ClassLib.Panel;

public record PanelRecord(
    PanelRecordKey PanelRecordKey,
    PanelTabKey ActivePanelTabKey,
    ElementDimensions ElementDimensions,
    ImmutableArray<PanelTab> PanelTabs);