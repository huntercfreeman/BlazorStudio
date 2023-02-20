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

    public Task CreateDirectoryAsync(
        string absoluteFilePathString,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(
        string absoluteFilePathString,
        bool recursive,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<bool> ExistsAsync(
        string absoluteFilePathString,
        CancellationToken cancellationToken = default)
    {
        Console.WriteLine(nameof(ExistsAsync));
        
        _httpClient.GetAsync("directoryExists");
        
        throw new NotImplementedException();
    }

    public Task MoveAsync(
        string sourceAbsoluteFilePathString,
        string destinationAbsoluteFilePathString,
        CancellationToken cancellationToken = default)
    {
        Console.WriteLine(nameof(MoveAsync));
        
        _httpClient.GetAsync("directoryMove");
        
        throw new NotImplementedException();
    }

    public Task<string[]> GetDirectoriesAsync(
        string absoluteFilePathString,
        CancellationToken cancellationToken = default)
    {
        Console.WriteLine(nameof(GetDirectoriesAsync));
        
        _httpClient.GetAsync("directoryGetDirectories");
        
        throw new NotImplementedException();
    }

    public Task<string[]> GetFilesAsync(
        string absoluteFilePathString,
        CancellationToken cancellationToken = default)
    {
        Console.WriteLine(nameof(GetFilesAsync));
        
        _httpClient.GetAsync("directoryGetFiles");
        
        throw new NotImplementedException();
    }

    public Task<IEnumerable<string>> EnumerateFileSystemEntriesAsync(
        string absoluteFilePathString,
        CancellationToken cancellationToken = default)
    {
        Console.WriteLine(nameof(EnumerateFileSystemEntriesAsync));

        _httpClient.GetAsync("directoryEnumerateFileSystemEntries");

        throw new NotImplementedException();
    }
}