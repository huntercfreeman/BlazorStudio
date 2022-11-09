using System.Collections.Immutable;

namespace BlazorStudio.ClassLib.Views;

public static class ViewFacts
{
    public static readonly View TerminalsView = new(
        ViewKey.NewViewKey("Terminals"), 
        ViewKind.Terminals);
    
    public static readonly View NugetPackageManagerView = new(
        ViewKey.NewViewKey("NuGet"), 
        ViewKind.NugetPackageManager);
    
    public static readonly ImmutableArray<View> Views = new[]
    {
        TerminalsView,
        NugetPackageManagerView
    }.ToImmutableArray();
}