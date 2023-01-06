using BlazorALaCarte.Shared.Dimensions;

namespace BlazorStudio.ClassLib.Panel;

public class PanelTab
{
    /// <summary>
    /// Each PanelTab maintains  its own element dimensions as
    /// each panel tab might need different amounts of space to be functionally usable.
    /// </summary>
    public ElementDimensions ElementDimensions { get; } = new();
    public Type ContentRendererType { get; init; }
    public Type IconRendererType { get; init; }
    public string DisplayName { get; init; }
}