using BlazorStudio.ClassLib.FileSystem.Interfaces;

namespace BlazorStudio.ClassLib.FileSystemApi.MemoryMapped;

public partial class FileSystemProvider : IFileSystemProvider, IDisposable
{
    private readonly Dictionary<AbsoluteFilePathStringValue, IFileHandle> _fileHandles = new();
    private readonly SemaphoreSlim _fileHandlesSemaphoreSlim = new(1, 1);

    public async Task<IFileHandle> OpenAsync(IAbsoluteFilePath absoluteFilePath, CancellationToken cancellationToken,
        FileHandleKind fileHandleKind)
    {
        var absoluteFilePathStringValue = new AbsoluteFilePathStringValue(absoluteFilePath);

        try
        {
            await _fileHandlesSemaphoreSlim.WaitAsync(cancellationToken);

            var fileHandle = 
                await UnsafePerformOpenAsync(absoluteFilePath, absoluteFilePathStringValue, fileHandleKind);
            
            _fileHandles.Add(absoluteFilePathStringValue, fileHandle);

            return fileHandle;
        }
        finally
        {
            _fileHandlesSemaphoreSlim.Release();
        }
    }

    private async Task<IFileHandle> UnsafePerformOpenAsync(IAbsoluteFilePath absoluteFilePath,
        AbsoluteFilePathStringValue filePathStringValue,
        FileHandleKind fileHandleKind)
    {
        var absoluteFilePathStringValue = new AbsoluteFilePathStringValue(absoluteFilePath);
        
        if (_fileHandles.TryGetValue(absoluteFilePathStringValue, out var value))
        {
            return value;
        }

        IFileHandle fileHandle;

        if (fileHandleKind == FileHandleKind.Tokenized)
        {
            fileHandle = new FileHandleTokenized(absoluteFilePath, Close);
        }
        else
        {
            var memoryMappedFileHandle = new FileHandleMemoryMapped(absoluteFilePath, Close);
            
            await memoryMappedFileHandle.InitializeAsync(CancellationToken.None);

            fileHandle = memoryMappedFileHandle;
        }

        return fileHandle;
    }

    private void Close(IFileHandle fileHandle)
    {
        var absoluteFilePathStringValue = new AbsoluteFilePathStringValue(fileHandle.AbsoluteFilePath);

        _ = Task.Run(async () =>
        {
            try
            {
                await _fileHandlesSemaphoreSlim.WaitAsync();

                _fileHandles.Remove(absoluteFilePathStringValue);
            }
            finally
            {
                _fileHandlesSemaphoreSlim.Release();
            }
        });
    }

    public void Dispose()
    {
        var localFileHandles = _fileHandles.Values
            .ToArray();
        
        foreach (var fileHandle in localFileHandles)
        {
            fileHandle.Dispose();
        }
    }
}