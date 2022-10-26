namespace BlazorStudio.ClassLib.NugetPackageManager;

public record NugetResponse(
    // ReSharper disable once NotAccessedPositionalProperty.Global
    int TotalHits,
    NugetPackageRecord[] Data);