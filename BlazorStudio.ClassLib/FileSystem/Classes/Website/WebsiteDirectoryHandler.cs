using BlazorStudio.ClassLib.Dto;
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

    public async Task CreateDirectoryAsync(
        string absoluteFilePathString,
        CancellationToken cancellationToken = default)
    {
        Console.WriteLine(nameof(CreateDirectoryAsync));
        
        await _httpClient.GetAsync(
            "https://hunter-freeman-dev-api.azurewebsites.net/" +
            "FileSystem/" +
            "DirectoryCreateDirectory?" +
            $"groupName={string.Empty}&" +
            $"absoluteFilePathString={absoluteFilePathString}",
            cancellationToken);
    }

    public async Task DeleteAsync(
        string absoluteFilePathString,
        bool recursive,
        CancellationToken cancellationToken = default)
    {
        Console.WriteLine(nameof(DeleteAsync));
        
        await _httpClient.GetAsync(
            "https://hunter-freeman-dev-api.azurewebsites.net/" +
            "FileSystem/" +
            "DirectoryDelete?" +
            $"groupName={string.Empty}&" +
            $"absoluteFilePathString={absoluteFilePathString}&" +
            $"recursive={recursive}",
            cancellationToken);
    }

    public async Task<bool> ExistsAsync(
        string absoluteFilePathString,
        CancellationToken cancellationToken = default)
    {
        Console.WriteLine(nameof(ExistsAsync));
        
        await _httpClient.GetAsync(
            "https://hunter-freeman-dev-api.azurewebsites.net/" +
            "FileSystem/" +
            "DirectoryExists?" +
            $"groupName={string.Empty}&" +
            $"absoluteFilePathString={absoluteFilePathString}",
            cancellationToken);

        return true;
    }

    public Task MoveAsync(
        string sourceAbsoluteFilePathString,
        string destinationAbsoluteFilePathString,
        CancellationToken cancellationToken = default)
    {
        Console.WriteLine(nameof(MoveAsync));
        
        throw new NotImplementedException();
    }

    public async Task<string[]> GetDirectoriesAsync(
        string absoluteFilePathString,
        CancellationToken cancellationToken = default)
    {
        Console.WriteLine(nameof(GetDirectoriesAsync));
        
        await _httpClient.GetAsync(
            "https://hunter-freeman-dev-api.azurewebsites.net/" +
            "FileSystem/" +
            "DirectoryGetDirectories?" +
            $"groupName={string.Empty}&" +
            $"absoluteFilePathString={absoluteFilePathString}",
            cancellationToken);

        return new[] { string.Empty };
    }

    public async Task<string[]> GetFilesAsync(
        string absoluteFilePathString,
        CancellationToken cancellationToken = default)
    {
        Console.WriteLine(nameof(GetFilesAsync));
        
        await _httpClient.GetAsync(
            "https://hunter-freeman-dev-api.azurewebsites.net/" +
            "FileSystem/" +
            "DirectoryGetFiles?" +
            $"groupName={string.Empty}&" +
            $"absoluteFilePathString={absoluteFilePathString}",
            cancellationToken);
        
        return new[] { string.Empty };
    }

    public async Task<IEnumerable<string>> EnumerateFileSystemEntriesAsync(
        string absoluteFilePathString,
        CancellationToken cancellationToken = default)
    {
        Console.WriteLine(nameof(EnumerateFileSystemEntriesAsync));

        await _httpClient.GetAsync(
            "https://hunter-freeman-dev-api.azurewebsites.net/" +
            "FileSystem/" +
            "DirectoryEnumerateFileSystemEntries?" +
            $"groupName={string.Empty}&" +
            $"absoluteFilePathString={absoluteFilePathString}",
            cancellationToken);

        return new[] { string.Empty };
    }
}