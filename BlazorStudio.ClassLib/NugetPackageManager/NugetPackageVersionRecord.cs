using System.Text.Json.Serialization;

namespace BlazorStudio.ClassLib.NugetPackageManager;

/// <summary>
/// When reading response Nuget returns <see cref="AtId" /> as a member named "@id"
/// </summary>
// ReSharper disable once ClassNeverInstantiated.Global
//
// Need disable ClassNeverInstantiated.Global because
// .GetFromJsonAsync<NugetResponse> is seemingly obfuscating
// the instantiation that is occurring.
public record NugetPackageVersionRecord(
    string Version,
    long Downloads)
{
    [JsonPropertyName("@id")]
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    //
    // Need disable UnusedAutoPropertyAccessor.Global because
    // Property is JSON deserialized
    // and I don't want to miss any of the data
    // because of a general sense of unease by the idea of that.
    public string? AtId { get; init; }
}