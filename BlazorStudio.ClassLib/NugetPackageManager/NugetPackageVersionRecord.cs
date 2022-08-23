using System.Text.Json.Serialization;

namespace BlazorStudio.ClassLib.NugetPackageManager;

/// <summary>
/// When reading response Nuget returns <see cref="AtId"/> as a member named "@id"
/// </summary>
public record NugetPackageVersionRecord(
    string Version,
    int Downloads)
{
    [JsonPropertyName("@id")] 
    public string AtId { get; init; }
}