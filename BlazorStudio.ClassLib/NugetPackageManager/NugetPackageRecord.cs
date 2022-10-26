using System.Collections.Immutable;
using System.Text.Json.Serialization;

namespace BlazorStudio.ClassLib.NugetPackageManager;

/// <summary>
/// When reading response Nuget returns <see cref="AtId" /> as a member named "@id"
/// </summary>
// ReSharper disable once ClassNeverInstantiated.Global
//
// Need disable UnusedAutoPropertyAccessor.Global because
// Property is JSON deserialized
// and the instantiation appears to not be picked up.
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
    long TotalDownloads,
    bool Verified,
    ImmutableArray<NugetPackageVersionRecord> Versions)
{
    [JsonPropertyName("@id")]
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    //
    // Need disable UnusedAutoPropertyAccessor.Global because
    // Property is JSON deserialized
    // and the instantiation appears to not be picked up.
    public string? AtId { get; init; }
    // TODO: Pull this data from the JSON but it seems to not be VITAL at this moment.
    // public ImmutableArray<string> PackageTypes { get; init; }
}