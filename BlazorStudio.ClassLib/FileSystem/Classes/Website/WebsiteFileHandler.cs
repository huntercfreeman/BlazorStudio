using BlazorStudio.ClassLib.Dto;
using BlazorStudio.ClassLib.FileSystem.Interfaces;
using BlazorStudio.ClassLib.Store.AccountCase;
using Fluxor;

namespace BlazorStudio.ClassLib.FileSystem.Classes.Website;

public class WebsiteFileHandler : IFileHandler
{
    private readonly IEnvironmentProvider _environmentProvider;
    private readonly IState<AccountState> _accountStateWrap;
    private readonly HttpClient _httpClient;

    public WebsiteFileHandler(
        IEnvironmentProvider environmentProvider,
        IState<AccountState> accountStateWrap,
        HttpClient httpClient)
    {
        _environmentProvider = environmentProvider;
        _accountStateWrap = accountStateWrap;
        _httpClient = httpClient;
    }

    public async Task<bool> ExistsAsync(
        string absoluteFilePathString,
        CancellationToken cancellationToken = default)
    {
        Console.WriteLine(nameof(ExistsAsync));
        
        await _httpClient.GetAsync(
            "https://hunter-freeman-dev-api.azurewebsites.net/FileSystem/DirectoryExists?groupName=default-group-name&absoluteFilePathString=Hunter%2FFreeman",
            cancellationToken);

        return true;
    }

    public Task DeleteAsync(string absoluteFilePathString, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task CopyAsync(
        string sourceAbsoluteFilePathString,
        string destinationAbsoluteFilePathString,
        CancellationToken cancellationToken = default)
    {
        Console.WriteLine(nameof(CopyAsync));
        
        await _httpClient.GetAsync(
            "fileCopy",
            cancellationToken);
    }

    public async Task MoveAsync(
        string sourceAbsoluteFilePathString,
        string destinationAbsoluteFilePathString,
        CancellationToken cancellationToken = default)
    {
        Console.WriteLine(nameof(MoveAsync));
        
        await _httpClient.GetAsync(
            "fileMove",
            cancellationToken);
        
        throw new NotImplementedException();
    }

    public async Task<DateTime> GetLastWriteTimeAsync(
        string absoluteFilePathString,
        CancellationToken cancellationToken = default)
    {
        Console.WriteLine(nameof(GetLastWriteTimeAsync));
        
        await _httpClient.GetAsync(
            "fileGetLastWriteTime",
            cancellationToken);
        
        throw new NotImplementedException();
    }

    public async Task<string> ReadAllTextAsync(
        string absoluteFilePathString,
        CancellationToken cancellationToken = default)
    {
        Console.WriteLine(nameof(ReadAllTextAsync));
        
        await _httpClient.GetAsync(
            "fileReadAllTextAsync",
            cancellationToken);
        
        throw new NotImplementedException();
    }

    public async Task WriteAllTextAsync(
        string absoluteFilePathString,
        string? contents,
        CancellationToken cancellationToken = default)
    {
        Console.WriteLine(nameof(WriteAllTextAsync));
        
        await _httpClient.GetAsync(
            "writeAllTextAsync",
            cancellationToken);
        
        throw new NotImplementedException();
    }
}