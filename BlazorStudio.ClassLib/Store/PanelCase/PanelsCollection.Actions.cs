using BlazorStudio.ClassLib.Panel;

namespace BlazorStudio.ClassLib.Store.PanelCase;

public partial record PanelsCollection
{
    public record RegisterPanelRecordAction(PanelRecord PanelRecord);
    public record DisposePanelRecordAction(PanelRecordKey PanelRecordKey);

    public record SetActivePanelTabAction(PanelRecordKey PanelRecordKey, PanelTabKey PanelTabKey);
}