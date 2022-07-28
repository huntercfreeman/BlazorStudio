using BlazorStudio.ClassLib.FileSystem.Interfaces;

namespace BlazorStudio.ClassLib.FileSystemApi;

public partial class FileSystemProvider : IFileSystemProvider
{
    public IFileHandle Open(IAbsoluteFilePath absoluteFilePath)
    {
        throw new NotImplementedException();
    }

    public Task<IFileHandle> OpenAsync(IAbsoluteFilePath absoluteFilePath, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public void Close(IFileHandle fileHandle)
    {
        throw new NotImplementedException();
    }

    public Task CloseAsync(IFileHandle fileHandle, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}