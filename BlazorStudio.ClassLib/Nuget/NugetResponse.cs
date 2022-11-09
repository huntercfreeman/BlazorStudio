namespace BlazorStudio.ClassLib.Nuget;

public record NugetResponse(
    int TotalHits,
    NugetPackageRecord[] Data);