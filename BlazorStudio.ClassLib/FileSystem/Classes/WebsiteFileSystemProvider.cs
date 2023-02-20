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
        
        var absoluteFilePathString = absoluteFilePath.GetAbsoluteFilePathString();
        
        absoluteFilePathString = FormatAbsoluteFilePathString(absoluteFilePathString);
        
        var accountState = _accountStateWrap.Value;
        
        var containerClient = _blobServiceClient.GetBlobContainerClient(
            accountState.ContainerName);

        GetBlobClient(
            containerClient,
            absoluteFilePathString,
            true,
            content);

        return Task.CompletedTask;
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
        
        var absoluteFilePathString = absoluteFilePath.GetAbsoluteFilePathString();
        
        absoluteFilePathString = FormatAbsoluteFilePathString(absoluteFilePathString);
        
        var accountState = _accountStateWrap.Value;
        
        var containerClient = _blobServiceClient.GetBlobContainerClient(
            accountState.ContainerName);

        _ = GetBlobClient(
            containerClient,
            absoluteFilePathString + DIRECTORY_FILE_NAME,
            true,
            string.Empty);

        return Task.CompletedTask;
    }

    public async Task DeleteFileAsync(
        IAbsoluteFilePath absoluteFilePath,
        CancellationToken cancellationToken = default)
    {
        Console.WriteLine(nameof(DeleteFileAsync));

        var absoluteFilePathString = absoluteFilePath.GetAbsoluteFilePathString();
        
        absoluteFilePathString = FormatAbsoluteFilePathString(absoluteFilePathString);
        
        var accountState = _accountStateWrap.Value;
        
        var containerClient = _blobServiceClient.GetBlobContainerClient(
            accountState.ContainerName);
        
        var blobClient = containerClient.GetBlobClient(absoluteFilePathString);

        await blobClient.DeleteAsync(cancellationToken: cancellationToken);
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
        absoluteFilePathString = FormatAbsoluteFilePathString(absoluteFilePathString);
        
        var accountState = _accountStateWrap.Value;
        
        var containerClient = _blobServiceClient.GetBlobContainerClient(
            accountState.ContainerName);
        
        var blobClient = containerClient.GetBlobClient(absoluteFilePathString);

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
        absoluteFilePathString = FormatAbsoluteFilePathString(absoluteFilePathString);
        
        Console.WriteLine(nameof(FileGetLastWriteTime));
        
        var accountState = _accountStateWrap.Value;
        
        var containerClient = _blobServiceClient.GetBlobContainerClient(
            accountState.ContainerName);

        var blobClient = containerClient.GetBlobClient(absoluteFilePathString);
        
        return blobClient
            .GetPropertiesAsync()
            .Result.Value.LastModified.UtcDateTime;
    }

    public async Task<string> FileReadAllTextAsync(
        string absoluteFilePathString)
    {
        absoluteFilePathString = FormatAbsoluteFilePathString(absoluteFilePathString);
        
        Console.WriteLine(nameof(FileReadAllTextAsync));
        
        var accountState = _accountStateWrap.Value;
        
        var containerClient = _blobServiceClient.GetBlobContainerClient(
            accountState.ContainerName);

        var blobClient = containerClient.GetBlobClient(absoluteFilePathString);
        
        var contentResponse = await blobClient.DownloadContentAsync();

        return System.Text.Encoding.UTF8.GetString(
            contentResponse.Value.Content);
    }

    public async Task WriteAllTextAsync(
        string absoluteFilePathString,
        string? contents,
        CancellationToken cancellationToken = default(CancellationToken))
    {
        absoluteFilePathString = FormatAbsoluteFilePathString(absoluteFilePathString);
        
        Console.WriteLine(nameof(WriteAllTextAsync));
        
        var accountState = _accountStateWrap.Value;
        
        var containerClient = _blobServiceClient.GetBlobContainerClient(
            accountState.ContainerName);

        var blobClient = containerClient.GetBlobClient(absoluteFilePathString);
        
        var stream = GenerateStreamFromString(contents ?? string.Empty);
            
        await blobClient.UploadAsync(
            stream, 
            true,
            cancellationToken);
    }

    public bool DirectoryExists(
        string absoluteFilePathString)
    {
        absoluteFilePathString = FormatAbsoluteFilePathString(absoluteFilePathString);
        
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
        absoluteFilePathString = FormatAbsoluteFilePathString(absoluteFilePathString);
        
        Console.WriteLine(nameof(DirectoryGetDirectories));
        
        var accountState = _accountStateWrap.Value;
        
        var containerClient = GetBlobContainerClient(accountState);

        var blobItemPages = containerClient
            .GetBlobs(prefix: absoluteFilePathString);

        var blobItems = blobItemPages
            .AsPages()
            .SelectMany(x => x.Values)
            .ToList();

        var directoryBlobs = blobItems
            .Where(blobItem => blobItem.Name
                .Replace(absoluteFilePathString, string.Empty)
                .Contains(_environmentProvider.DirectorySeparatorChar));

        var childBlobs = directoryBlobs
            .Where(blobItem =>
            {
                var splitName = blobItem.Name
                    .Split(_environmentProvider.DirectorySeparatorChar);

                var inputSplitLength = absoluteFilePathString
                    .Split(_environmentProvider.DirectorySeparatorChar)
                    .Length;

                if (splitName.Length > inputSplitLength + 1)
                    return false;

                if (blobItem.Name.EndsWith(DIRECTORY_FILE_NAME))
                    return true;

                return false;
            });

        var result = childBlobs
            .Select(x => x.Name
                .Replace(
                    _environmentProvider.DirectorySeparatorChar + DIRECTORY_FILE_NAME,
                    string.Empty))
            .ToArray();
        
        return result;
    }

    public string[] DirectoryGetFiles(
        string absoluteFilePathString)
    {
        absoluteFilePathString = FormatAbsoluteFilePathString(absoluteFilePathString);
        
        Console.WriteLine(nameof(DirectoryGetFiles));
        
        var accountState = _accountStateWrap.Value;
        
        var containerClient = GetBlobContainerClient(accountState);

        var blobItemPages = containerClient
            .GetBlobs(prefix: absoluteFilePathString);

        var blobItems = blobItemPages
            .AsPages()
            .SelectMany(x => x.Values)
            .ToList();

        return blobItems
            .Where(blobItem => !blobItem.Name
                .Replace(absoluteFilePathString, string.Empty)
                .Contains(_environmentProvider.DirectorySeparatorChar) &&
                               !blobItem.Name.EndsWith(DIRECTORY_FILE_NAME))
            .Select(x => x.Name)
            .ToArray();
    }

    private string FormatAbsoluteFilePathString(string absoluteFilePathString)
    {
        if (absoluteFilePathString.StartsWith(_environmentProvider.DirectorySeparatorChar))
        {
            return new string(absoluteFilePathString
                .Skip(1)
                .ToArray());
        }

        return absoluteFilePathString;
    }

    public IEnumerable<string> DirectoryEnumerateFileSystemEntries(
        string absoluteFilePathString)
    {
        Console.WriteLine(nameof(DirectoryEnumerateFileSystemEntries));

        var childDirectories = DirectoryGetDirectories(absoluteFilePathString);
        var childFiles = DirectoryGetFiles(absoluteFilePathString);

        return childDirectories.Union(childFiles);
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
        bool createIfDoesNotExist,
        string createIfDoesNotExistInitialText)
    {
        var blobClient = blobContainerClient.GetBlobClient(absoluteFilePathString);

        if (!blobClient.Exists() &&
            createIfDoesNotExist)
        {
            var stream = GenerateStreamFromString(createIfDoesNotExistInitialText);

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