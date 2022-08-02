using System.IO.MemoryMappedFiles;
using BlazorStudio.ClassLib.FileSystem.Interfaces;

namespace BlazorStudio.ClassLib.FileSystemApi;

public interface IFileSystemProvider
{
    /// <summary>
    /// Get a file handle to a modifiable extremely large file without reading the entirety of the file into memory. 
    /// </summary>
    /// <param name="absoluteFilePath">
    /// An absolute path to a file, but typed as such to avoid confusion between Absolute and Relative paths.
    /// </param>
    /// <param name="cancellationToken">Relays the cancellation of the asynchronous call</param>
    /// <returns>
    /// A Task that wraps a reference to an abstraction for a modifiable <see cref="MemoryMappedFile"/>
    /// </returns>
    public Task<IFileHandle> OpenAsync(IAbsoluteFilePath absoluteFilePath, CancellationToken cancellationToken);
}