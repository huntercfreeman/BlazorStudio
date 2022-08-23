using System.Collections.Immutable;

namespace BlazorStudio.ClassLib.NugetPackageManager;

public interface INugetPackageManagerProvider
{
    public string ProviderWebsiteUrlNoFormatting { get; }

    public Task<ImmutableArray<NugetPackageRecord>> QueryForNugetPackagesAsync(
        string query,
        bool includePrerelease = false,
        CancellationToken cancellationToken = default);

    public Task<ImmutableArray<NugetPackageRecord>> QueryForNugetPackagesAsync(
        NugetPackageManagerQuery nugetPackageManagerQuery,
        CancellationToken cancellationToken = default);

    public NugetPackageManagerQuery BuildQuery(string query, bool includePrerelease = false);
}