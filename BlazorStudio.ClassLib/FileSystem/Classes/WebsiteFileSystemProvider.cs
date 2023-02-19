using Azure.Storage.Blobs;
using BlazorStudio.ClassLib.FileSystem.Interfaces;
using BlazorStudio.ClassLib.Store.AccountCase;
using Fluxor;

namespace BlazorStudio.ClassLib.FileSystem.Classes;

public class WebsiteFileSystemProvider : IFileSystemProvider
{
    private const string AZURE_STORAGE_CONNECTION_STRING_ENVIRONMENT_VARIABLE_NAME = "AZURE_STORAGE_CONNECTION_STRING";

    private readonly IEnvironmentProvider _environmentProvider;
    private readonly IState<AccountState> _accountStateWrap;
    
    private readonly BlobServiceClient _blobServiceClient = new(
        Environment.GetEnvironmentVariable(
            AZURE_STORAGE_CONNECTION_STRING_ENVIRONMENT_VARIABLE_NAME));

    public WebsiteFileSystemProvider(
        IEnvironmentProvider environmentProvider,
        IState<AccountState> accountStateWrap)
    {
        _environmentProvider = environmentProvider;
        _accountStateWrap = accountStateWrap;
    }

    public Task WriteFileAsync(
        IAbsoluteFilePath absoluteFilePath, 
        string content,
        bool overwrite, 
        bool create,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<string> ReadFileAsync(
        IAbsoluteFilePath absoluteFilePath, 
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task CreateDirectoryAsync(IAbsoluteFilePath absoluteFilePath, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task DeleteFileAsync(IAbsoluteFilePath absoluteFilePath, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
    
    public Task DeleteDirectoryAsync(IAbsoluteFilePath absoluteFilePath, bool recursive, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
    
    public bool FileExists(string absoluteFilePathString)
    {
        var accountState = _accountStateWrap.Value;
        
        var containerClient = _blobServiceClient.GetBlobContainerClient(
            accountState.ContainerName);

        var blobName = _environmentProvider.RootDirectoryAbsoluteFilePath + 
                       absoluteFilePathString;
        
        var blobClient = containerClient.GetBlobClient(blobName);

        return blobClient.Exists();
    }

    public void FileCopy(string sourceAbsoluteFilePathString, string destinationAbsoluteFilePathString)
    {
        throw new NotImplementedException();
    }

    public void FileMove(string sourceAbsoluteFilePathString, string destinationAbsoluteFilePathString)
    {
        throw new NotImplementedException();
    }

    public DateTime FileGetLastWriteTime(string absoluteFilePathString)
    {
        throw new NotImplementedException();
    }

    public Task<string> FileReadAllTextAsync(string absoluteFilePathString)
    {
        throw new NotImplementedException();
    }

    public Task WriteAllTextAsync(string absoluteFilePathString, string? contents, CancellationToken cancellationToken = default(CancellationToken))
    {
        throw new NotImplementedException();
    }

    public bool DirectoryExists(string absoluteFilePathString)
    {
        throw new NotImplementedException();
    }

    public void DirectoryMove(string sourceAbsoluteFilePathString, string destinationAbsoluteFilePathString)
    {
        throw new NotImplementedException();
    }

    public string[] DirectoryGetDirectories(string absoluteFilePathString)
    {
        throw new NotImplementedException();
    }

    public string[] DirectoryGetFiles(string absoluteFilePathString)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<string> DirectoryEnumerateFileSystemEntries(string absoluteFilePathString)
    {
        throw new NotImplementedException();
    }
}