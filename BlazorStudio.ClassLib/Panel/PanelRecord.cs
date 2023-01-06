using System.Collections.Immutable;

namespace BlazorStudio.ClassLib.Panel;

public record PanelRecord(
    PanelRecordKey PanelRecordKey,
    PanelTabKey ActivePanelTabKey,
    ImmutableArray<PanelTab> PanelTabs);