using BlazorStudio.ClassLib.Dto;
using BlazorStudio.ClassLib.FileSystem.Interfaces;
using BlazorStudio.ClassLib.Store.AccountCase;
using BlazorStudio.ClassLib.Store.FileSystemCase;
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
            "https://hunter-freeman-dev-api.azurewebsites.net/" +
            "FileSystem/" +
            "FileExists?" +
            $"groupName={string.Empty}&" +
            $"absoluteFilePathString={absoluteFilePathString}",
            cancellationToken);

        return true;
    }

    public async Task DeleteAsync(string absoluteFilePathString, CancellationToken cancellationToken = default)
    {
        Console.WriteLine(nameof(DeleteAsync));
        
        await _httpClient.GetAsync(
            "https://hunter-freeman-dev-api.azurewebsites.net/" +
            "FileSystem/" +
            "FileDelete?" +
            $"groupName={string.Empty}&" +
            $"absoluteFilePathString={absoluteFilePathString}",
            cancellationToken);
    }

    public async Task CopyAsync(
        string sourceAbsoluteFilePathString,
        string destinationAbsoluteFilePathString,
        CancellationToken cancellationToken = default)
    {
        Console.WriteLine(nameof(CopyAsync));

        throw new NotImplementedException();
    }

    public async Task MoveAsync(
        string sourceAbsoluteFilePathString,
        string destinationAbsoluteFilePathString,
        CancellationToken cancellationToken = default)
    {
        Console.WriteLine(nameof(MoveAsync));

        throw new NotImplementedException();
    }

    public async Task<DateTime> GetLastWriteTimeAsync(
        string absoluteFilePathString,
        CancellationToken cancellationToken = default)
    {
        Console.WriteLine(nameof(GetLastWriteTimeAsync));
        
        await _httpClient.GetAsync(
            "https://hunter-freeman-dev-api.azurewebsites.net/" +
            "FileSystem/" +
            "FileGetLastWriteTime?" +
            $"groupName={string.Empty}&" +
            $"absoluteFilePathString={absoluteFilePathString}",
            cancellationToken);

        return DateTime.UtcNow;
    }

    public async Task<string> ReadAllTextAsync(
        string absoluteFilePathString,
        CancellationToken cancellationToken = default)
    {
        Console.WriteLine(nameof(ReadAllTextAsync));
        
        await _httpClient.GetAsync(
            "https://hunter-freeman-dev-api.azurewebsites.net/" +
            "FileSystem/" +
            "FileReadAllText?" +
            $"groupName={string.Empty}&" +
            $"absoluteFilePathString={absoluteFilePathString}",
            cancellationToken);

        return string.Empty;
    }

    public async Task WriteAllTextAsync(
        string absoluteFilePathString,
        string contents,
        CancellationToken cancellationToken = default)
    {
        Console.WriteLine(nameof(WriteAllTextAsync));

        if (contents.Length > FileSystemState.MAXIMUM_CHARACTER_COUNT_OF_CONTENT)
        {
            contents = new string(contents
                .Take(FileSystemState.MAXIMUM_CHARACTER_COUNT_OF_CONTENT)
                .ToArray());
        }
        
        await _httpClient.GetAsync(
            "https://hunter-freeman-dev-api.azurewebsites.net/" +
            "FileSystem/" +
            "FileWriteAllText?" +
            $"groupName={string.Empty}&" +
            $"absoluteFilePathString={absoluteFilePathString}&" +
            $"contents={contents}",
            cancellationToken);
    }
}