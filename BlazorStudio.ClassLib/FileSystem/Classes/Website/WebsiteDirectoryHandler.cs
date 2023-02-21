using System.Net.Http.Json;
using System.Web;
using BlazorStudio.ClassLib.Dto;
using BlazorStudio.ClassLib.FileSystem.Classes.FilePath;
using BlazorStudio.ClassLib.FileSystem.Interfaces;
using BlazorStudio.ClassLib.Store.AccountCase;
using Fluxor;

namespace BlazorStudio.ClassLib.FileSystem.Classes.Website;

public class WebsiteDirectoryHandler : IDirectoryHandler
{
    private readonly IEnvironmentProvider _environmentProvider;
    private readonly IState<AccountState> _accountStateWrap;
    private readonly HttpClient _httpClient;

    public WebsiteDirectoryHandler(
        IEnvironmentProvider environmentProvider,
        IState<AccountState> accountStateWrap,
        HttpClient httpClient)
    {
        _environmentProvider = environmentProvider;
        _accountStateWrap = accountStateWrap;
        _httpClient = httpClient;
    }

    public Task CreateDirectoryAsync(
        string absoluteFilePathString,
        CancellationToken cancellationToken = default)
    {
        Console.WriteLine(nameof(CreateDirectoryAsync));
        
        absoluteFilePathString = PathFormatter.FormatAbsoluteFilePathString(
            absoluteFilePathString,
            AccountState.DIRECTORY_SEPARATOR_CHAR,
            true);
        
        _httpClient.GetAsync(
            "https://hunter-freeman-dev-api.azurewebsites.net/" +
            "FileSystem/" +
            "DirectoryCreateDirectory?" +
            $"absoluteFilePathString={Uri.EscapeDataString(absoluteFilePathString)}",
            cancellationToken)
            .Wait(cancellationToken);

        return Task.CompletedTask;
    }

    public Task DeleteAsync(
        string absoluteFilePathString,
        bool recursive,
        CancellationToken cancellationToken = default)
    {
        Console.WriteLine(nameof(DeleteAsync));
        
        absoluteFilePathString = PathFormatter.FormatAbsoluteFilePathString(
            absoluteFilePathString,
            AccountState.DIRECTORY_SEPARATOR_CHAR,
            true);
        
        _httpClient.GetAsync(
            "https://hunter-freeman-dev-api.azurewebsites.net/" +
            "FileSystem/" +
            "DirectoryDelete?" +
            $"absoluteFilePathString={Uri.EscapeDataString(absoluteFilePathString)}&" +
            $"recursive={recursive}",
            cancellationToken)
            .Wait(cancellationToken);
        
        return Task.CompletedTask;
    }

    public Task<bool> ExistsAsync(
        string absoluteFilePathString,
        CancellationToken cancellationToken = default)
    {
        Console.WriteLine(nameof(ExistsAsync));
        
        absoluteFilePathString = PathFormatter.FormatAbsoluteFilePathString(
            absoluteFilePathString,
            AccountState.DIRECTORY_SEPARATOR_CHAR,
            true);
        
        var response = _httpClient.GetFromJsonAsync<bool>(
            "https://hunter-freeman-dev-api.azurewebsites.net/" +
            "FileSystem/" +
            "DirectoryExists?" +
            $"absoluteFilePathString={Uri.EscapeDataString(absoluteFilePathString)}",
            cancellationToken)
            .Result;

        return Task.FromResult(
            response);
    }

    public Task MoveAsync(
        string sourceAbsoluteFilePathString,
        string destinationAbsoluteFilePathString,
        CancellationToken cancellationToken = default)
    {
        Console.WriteLine(nameof(MoveAsync));
        
        throw new NotImplementedException();
    }

    public Task<string[]> GetDirectoriesAsync(
        string absoluteFilePathString,
        CancellationToken cancellationToken = default)
    {
        Console.WriteLine(nameof(GetDirectoriesAsync));
        
        absoluteFilePathString = PathFormatter.FormatAbsoluteFilePathString(
            absoluteFilePathString,
            AccountState.DIRECTORY_SEPARATOR_CHAR,
            true);

        var requestUri = "https://hunter-freeman-dev-api.azurewebsites.net/" +
                         "FileSystem/" +
                         "DirectoryGetDirectories?" +
                         $"absoluteFilePathString={Uri.EscapeDataString(absoluteFilePathString)}";

        var response = _httpClient.GetFromJsonAsync<string[]>(
                requestUri,
                cancellationToken)
            .Result;

        return Task.FromResult(
            response ?? new[] { string.Empty });
    }

    public Task<string[]> GetFilesAsync(
        string absoluteFilePathString,
        CancellationToken cancellationToken = default)
    {
        Console.WriteLine(nameof(GetFilesAsync));
        
        absoluteFilePathString = PathFormatter.FormatAbsoluteFilePathString(
            absoluteFilePathString,
            AccountState.DIRECTORY_SEPARATOR_CHAR,
            true);
        
        var response = _httpClient.GetFromJsonAsync<string[]>(
            "https://hunter-freeman-dev-api.azurewebsites.net/" +
            "FileSystem/" +
            "DirectoryGetFiles?" +
            $"absoluteFilePathString={Uri.EscapeDataString(absoluteFilePathString)}",
            cancellationToken)
            .Result;
        
        return Task.FromResult(
            response ?? new[] { string.Empty });
    }

    public Task<IEnumerable<string>> EnumerateFileSystemEntriesAsync(
        string absoluteFilePathString,
        CancellationToken cancellationToken = default)
    {
        Console.WriteLine(nameof(EnumerateFileSystemEntriesAsync));
        
        absoluteFilePathString = PathFormatter.FormatAbsoluteFilePathString(
            absoluteFilePathString,
            AccountState.DIRECTORY_SEPARATOR_CHAR,
            true);

        var response = _httpClient.GetFromJsonAsync<string[]>(
            "https://hunter-freeman-dev-api.azurewebsites.net/" +
            "FileSystem/" +
            "DirectoryEnumerateFileSystemEntries?" +
            $"absoluteFilePathString={Uri.EscapeDataString(absoluteFilePathString)}",
            cancellationToken)
            .Result;

        return Task.FromResult<IEnumerable<string>>(
            response ?? new[] { string.Empty });
    }
}