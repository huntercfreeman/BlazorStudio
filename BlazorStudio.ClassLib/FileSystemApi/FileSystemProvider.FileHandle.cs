using BlazorStudio.ClassLib.FileSystem.Interfaces;

namespace BlazorStudio.ClassLib.FileSystemApi;

public partial class FileSystemProvider : IFileSystemProvider
{
    private class FileHandle : IFileHandle
    {
        public IAbsoluteFilePath AbsoluteFilePath { get; }
    
        public void Save()
        {
            throw new NotImplementedException();
        }
        
        public Task SaveAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
        
        public string Read(long rowOffset, long characterOffset)
        {
            throw new NotImplementedException();
        }
        
        public Task<string> ReadAsync(long rowOffset, long characterOffset, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
        
        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}