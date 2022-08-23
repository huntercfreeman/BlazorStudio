using System.Collections.Immutable;
using System.Text.Json.Serialization;

namespace BlazorStudio.ClassLib.NugetPackageManager;

/// <summary>
/// When reading response Nuget returns <see cref="AtId"/> as a member named "@id"
/// </summary>
public record NugetPackageRecord(
    string Type,
    string Registration,
    string Id,
    string Version,
    string Description,
    string Summary,
    string Title,
    string IconUrl,
    string LicenseUrl,
    string ProjectUrl,
    ImmutableArray<string> Tags,
    ImmutableArray<string> Authors,
    ImmutableArray<string> Owners,
    int TotalDownloads,
    bool Verified,
    ImmutableArray<string> PackageTypes,
    ImmutableArray<NugetPackageVersionRecord> Versions)
{
    [JsonPropertyName("@id")] 
    public string AtId { get; init; }
}