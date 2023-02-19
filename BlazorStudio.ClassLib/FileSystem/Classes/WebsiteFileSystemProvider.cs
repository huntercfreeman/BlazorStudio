using System.Text;
using Azure.Storage.Blobs;
using BlazorStudio.ClassLib.FileSystem.Interfaces;
using BlazorStudio.ClassLib.Store.AccountCase;
using Fluxor;

namespace BlazorStudio.ClassLib.FileSystem.Classes;

public class WebsiteFileSystemProvider : IFileSystemProvider
{
    private const string AZURE_STORAGE_CONNECTION_STRING_ENVIRONMENT_VARIABLE_NAME = "AZURE_STORAGE_CONNECTION_STRING";
    private const string DIRECTORY_FILE_NAME = "dir";
    
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
        Console.WriteLine(nameof(WriteFileAsync));
        
        throw new NotImplementedException();
    }

    public Task<string> ReadFileAsync(
        IAbsoluteFilePath absoluteFilePath, 
        CancellationToken cancellationToken = default)
    {
        Console.WriteLine(nameof(ReadFileAsync));
        
        throw new NotImplementedException();
    }

    public Task CreateDirectoryAsync(
        IAbsoluteFilePath absoluteFilePath,
        CancellationToken cancellationToken = default)
    {
        Console.WriteLine(nameof(CreateDirectoryAsync));
        
        throw new NotImplementedException();
    }

    public Task DeleteFileAsync(
        IAbsoluteFilePath absoluteFilePath,
        CancellationToken cancellationToken = default)
    {
        Console.WriteLine(nameof(DeleteFileAsync));
        
        throw new NotImplementedException();
    }
    
    public Task DeleteDirectoryAsync(
        IAbsoluteFilePath absoluteFilePath,
        bool recursive,
        CancellationToken cancellationToken = default)
    {
        Console.WriteLine(nameof(DeleteDirectoryAsync));
        
        throw new NotImplementedException();
    }
    
    public bool FileExists(
        string absoluteFilePathString)
    {
        var accountState = _accountStateWrap.Value;
        
        var containerClient = _blobServiceClient.GetBlobContainerClient(
            accountState.ContainerName);

        var blobName = _environmentProvider.RootDirectoryAbsoluteFilePath + 
                       absoluteFilePathString;
        
        var blobClient = containerClient.GetBlobClient(blobName);

        return blobClient.Exists();
    }

    public void FileCopy(
        string sourceAbsoluteFilePathString,
        string destinationAbsoluteFilePathString)
    {
        Console.WriteLine(nameof(FileCopy));
        
        throw new NotImplementedException();
    }

    public void FileMove(
        string sourceAbsoluteFilePathString,
        string destinationAbsoluteFilePathString)
    {
        Console.WriteLine(nameof(FileMove));
        
        throw new NotImplementedException();
    }

    public DateTime FileGetLastWriteTime(
        string absoluteFilePathString)
    {
        Console.WriteLine(nameof(FileGetLastWriteTime));
        
        throw new NotImplementedException();
    }

    public Task<string> FileReadAllTextAsync(
        string absoluteFilePathString)
    {
        Console.WriteLine(nameof(FileReadAllTextAsync));
        
        throw new NotImplementedException();
    }

    public Task WriteAllTextAsync(
        string absoluteFilePathString,
        string? contents,
        CancellationToken cancellationToken = default(CancellationToken))
    {
        Console.WriteLine(nameof(WriteAllTextAsync));
        
        throw new NotImplementedException();
    }

    public bool DirectoryExists(
        string absoluteFilePathString)
    {
        Console.WriteLine(nameof(DirectoryExists));
        
        var accountState = _accountStateWrap.Value;
        
        var containerClient = GetBlobContainerClient(accountState);
        
        var blobClient = containerClient.GetBlobClient(absoluteFilePathString);

        return blobClient.Exists();
    }

    public void DirectoryMove(
        string sourceAbsoluteFilePathString,
        string destinationAbsoluteFilePathString)
    {
        Console.WriteLine(nameof(DirectoryMove));
        
        throw new NotImplementedException();
    }

    public string[] DirectoryGetDirectories(
        string absoluteFilePathString)
    {
        Console.WriteLine(nameof(DirectoryGetDirectories));
        
        var accountState = _accountStateWrap.Value;
        
        var containerClient = GetBlobContainerClient(accountState);

        var directoryBlobClient = GetBlobClient(
            containerClient,
            absoluteFilePathString + DIRECTORY_FILE_NAME,
            true);
        
        var blobItemPages = containerClient
            .GetBlobs(prefix: absoluteFilePathString);

        var blobItems = blobItemPages
            .AsPages()
            .SelectMany(x => x.Values)
            .Where(blobItem => blobItem.Name
                .Replace(absoluteFilePathString, string.Empty)
                .Contains(_environmentProvider.DirectorySeparatorChar))
            .ToList();

        return blobItems
            .Select(x => x.Name)
            .ToArray();
    }

    public string[] DirectoryGetFiles(
        string absoluteFilePathString)
    {
        Console.WriteLine(nameof(DirectoryGetFiles));
        
        var accountState = _accountStateWrap.Value;
        
        var containerClient = GetBlobContainerClient(accountState);

        var directoryBlobClient = GetBlobClient(
            containerClient,
            absoluteFilePathString + DIRECTORY_FILE_NAME,
            true);
        
        var blobItemPages = containerClient
            .GetBlobs(prefix: absoluteFilePathString);

        var blobItems = blobItemPages
            .AsPages()
            .SelectMany(x => x.Values)
            .Where(blobItem => !blobItem.Name
                .Replace(absoluteFilePathString, string.Empty)
                .Contains(_environmentProvider.DirectorySeparatorChar))
            .ToList();

        return blobItems
            .Select(x => x.Name)
            .ToArray();
    }

    public IEnumerable<string> DirectoryEnumerateFileSystemEntries(
        string absoluteFilePathString)
    {
        Console.WriteLine(nameof(DirectoryEnumerateFileSystemEntries));
        
        throw new NotImplementedException();
    }
    
    private BlobContainerClient GetBlobContainerClient(
        AccountState accountState)
    {
        var blobContainerClient = _blobServiceClient.GetBlobContainerClient(
            accountState.ContainerName);

        blobContainerClient.CreateIfNotExists();

        return blobContainerClient;
    }
    
    private BlobClient GetBlobClient(
        BlobContainerClient blobContainerClient,
        string absoluteFilePathString,
        bool createIfDoesNotExist)
    {
        var blobClient = blobContainerClient.GetBlobClient(absoluteFilePathString);

        if (!blobClient.Exists() &&
            createIfDoesNotExist)
        {
            var stream = GenerateStreamFromString(string.Empty);

            blobClient
                .UploadAsync(stream, true)
                .Wait();
        }

        return blobClient;
    }
    
    public Stream GenerateStreamFromString(
        string s)
    {
        return new MemoryStream(Encoding.UTF8.GetBytes(s));
    }
}