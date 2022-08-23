namespace BlazorStudio.ClassLib.NugetPackageManager;

public record NugetResponse(
    int TotalHits,
    NugetPackageRecord[] Data);