using System.Collections.Immutable;
using System.Net.Http.Json;
using System.Text;
using System.Web;

namespace BlazorStudio.ClassLib.NugetPackageManager;

public class NugetPackageManagerProviderAzureSearchUsnc : INugetPackageManagerProvider
{
    private readonly HttpClient _httpClient;

    public NugetPackageManagerProviderAzureSearchUsnc(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public string ProviderWebsiteUrlNoFormatting { get; } = "https://azuresearch-usnc.nuget.org/";

    public async Task<ImmutableArray<NugetPackageRecord>> QueryForNugetPackagesAsync(
        string queryValue,
        bool includePrerelease = false,
        CancellationToken cancellationToken = default)
    {
        return await QueryForNugetPackagesAsync(
            BuildQuery(queryValue, includePrerelease),
            cancellationToken);
    }

    public async Task<ImmutableArray<NugetPackageRecord>> QueryForNugetPackagesAsync(
        INugetPackageManagerQuery nugetPackageManagerQuery,
        CancellationToken cancellationToken = default)
    {
        var query = nugetPackageManagerQuery.Query;

        var response = await _httpClient.PostAsync(query, null);
        var debugging = await response.Content.ReadAsStringAsync();

        var nugetPackages = await _httpClient
            .GetFromJsonAsync<NugetResponse>(
                query,
                cancellationToken);

        return nugetPackages.Data.ToImmutableArray();
    }

    public INugetPackageManagerQuery BuildQuery(string query, bool includePrerelease = false)
    {
        var queryBuilder = new StringBuilder(ProviderWebsiteUrlNoFormatting + "query?");

        queryBuilder.Append($"q={HttpUtility.UrlEncode(query)}");

        queryBuilder.Append('&');

        queryBuilder.Append($"prerelease={includePrerelease}");

        return new NugetPackageManagerQuery(queryBuilder.ToString());
    }

    private record NugetPackageManagerQuery(string Query) : INugetPackageManagerQuery;
}