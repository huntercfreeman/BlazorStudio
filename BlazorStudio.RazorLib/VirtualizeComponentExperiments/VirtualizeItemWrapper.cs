namespace BlazorStudio.RazorLib.VirtualizeComponentExperiments;

public record VirtualizeItemWrapper<TItem>(TItem Item, double TopInPixels, double WidthInPercentage);