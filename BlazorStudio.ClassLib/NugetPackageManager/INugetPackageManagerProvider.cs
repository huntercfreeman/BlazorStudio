using System.Collections.Immutable;

namespace BlazorStudio.ClassLib.NugetPackageManager;

public interface INugetPackageManagerProvider
{
    public string ProviderWebsiteUrlNoFormatting { get; }
}

public class NugetPackageManagerProviderAzureSearchUsnc : INugetPackageManagerProvider
{
    private const string _formatStringWebsiteUrl = "https://azuresearch-usnc.nuget.org/query?q=${nugetQuery}&prerelease=${includePrerelease}";

    public string ProviderWebsiteUrlNoFormatting { get; } = "https://azuresearch-usnc.nuget.org/";
}

/// <summary>
/// When reading response Nuget returns <see cref="AtId"/> as a member named "@id"
/// </summary>
public record NugetPackageRecord(
    string AtId,
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
    ImmutableArray<NugetPackageVersionRecord> Versions);

/// <summary>
/// When reading response Nuget returns <see cref="AtId"/> as a member named "@id"
/// </summary>
public record NugetPackageVersionRecord(
    string Version,
    int Downloads,
    string AtId);