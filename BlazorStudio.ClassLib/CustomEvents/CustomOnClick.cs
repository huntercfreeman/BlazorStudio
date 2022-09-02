namespace BlazorStudio.RazorLib.CustomEvents;

public class CustomOnClick : EventArgs
{
    public double ClientX { get; set; }
    public double ClientY { get; set; }
    public double OffsetX { get; set; }
    public double OffsetY { get; set; }
    public string TargetElementId { get; set; } = "null";
    public string CurrentTargetId { get; set; } = "null";
    public string OffsetParentId { get; set; } = "null";
}