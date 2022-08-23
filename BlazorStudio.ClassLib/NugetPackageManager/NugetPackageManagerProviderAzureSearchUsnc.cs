using System.Collections.Immutable;
using System.Net.Http.Json;
using System.Text;

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
        NugetPackageManagerQuery nugetPackageManagerQuery, 
        CancellationToken cancellationToken = default)
    {
        var query = nugetPackageManagerQuery.Query;
        
        var nugetPackages = await _httpClient
            .GetFromJsonAsync<List<NugetPackageRecord>>(
                query, 
                cancellationToken: cancellationToken);

        return nugetPackages.ToImmutableArray();
    }
    
    public NugetPackageManagerQuery BuildQuery(string query, bool includePrerelease = false)
    {
        var queryBuilder = new StringBuilder(ProviderWebsiteUrlNoFormatting + "query?");

        queryBuilder.Append($"q={query}");
        
        queryBuilder.Append('&');
        
        queryBuilder.Append($"prerelease={includePrerelease}");
        
        return new(queryBuilder.ToString());
    }
}