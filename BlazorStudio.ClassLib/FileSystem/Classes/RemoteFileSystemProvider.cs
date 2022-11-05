using BlazorStudio.ClassLib.FileSystem.Interfaces;

namespace BlazorStudio.ClassLib.FileSystem.Classes;

public class RemoteFileSystemProvider : IFileSystemProvider
{
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
}