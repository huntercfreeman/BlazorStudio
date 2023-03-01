using System.Collections.Immutable;
using BlazorCommon.RazorLib.Dimensions;

namespace BlazorStudio.ClassLib.Panel;

public record PanelRecord(
    PanelRecordKey PanelRecordKey,
    PanelTabKey ActivePanelTabKey,
    ElementDimensions ElementDimensions,
    ImmutableArray<PanelTab> PanelTabs);