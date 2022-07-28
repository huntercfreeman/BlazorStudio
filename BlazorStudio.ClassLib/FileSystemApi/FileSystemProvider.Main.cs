using System.Collections.Concurrent;
using BlazorStudio.ClassLib.FileSystem.Interfaces;

namespace BlazorStudio.ClassLib.FileSystemApi;

public partial class FileSystemProvider : IFileSystemProvider, IDisposable
{
    private readonly Dictionary<AbsoluteFilePathStringValue, IFileHandle> _fileHandles = new();

    public IFileHandle Open(IAbsoluteFilePath absoluteFilePath)
    {
        var fileHandle = new FileHandle(absoluteFilePath);
        
        _fileHandles.Add(fileHandle);
    }

    public Task<IFileHandle> OpenAsync(IAbsoluteFilePath absoluteFilePath, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    private void Close(IFileHandle fileHandle)
    {
        _fileHandles.
    }

    public void Dispose()
    {
        var localFileHandles = _fileHandles.ToArray();
        
        foreach (var fileHandle in localFileHandles)
        {
            fileHandle.Dispose();
        }
    }
}