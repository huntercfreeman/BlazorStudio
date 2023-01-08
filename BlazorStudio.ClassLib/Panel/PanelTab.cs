using BlazorALaCarte.Shared.Dimensions;

namespace BlazorStudio.ClassLib.Panel;

/// <summary>
/// Each PanelTab maintains  its own element dimensions as
/// each panel tab might need different amounts of space to be functionally usable.
/// </summary>
public record PanelTab(
    PanelTabKey PanelTabKey,
    ElementDimensions ElementDimensions,
    Type ContentRendererType,
    Type IconRendererType,
    string DisplayName);