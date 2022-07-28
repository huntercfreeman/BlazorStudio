using BlazorStudio.ClassLib.FileSystem.Interfaces;

namespace BlazorStudio.ClassLib.FileSystemApi;

public interface IFileHandle : IDisposable
{
    /// <summary>
    /// A unique identifier for the file handle
    /// </summary>
    public FileHandleKey FileHandleKey { get; }
    /// <summary>
    /// Mostly this is a string that is an absolute path to a file.
    /// However, it is important that Absolute vs Relative
    /// paths are unambiguous  so this is typed accordingly.
    /// </summary>
    public IAbsoluteFilePath AbsoluteFilePath { get; }
    
    /// <summary>
    /// Write out any changes pending in memory to the FileSystem
    /// </summary>
    public void Save();
    /// <summary>
    /// Write out any changes pending in memory to the FileSystem
    /// </summary>
    /// <param name="cancellationToken">Relays the cancellation of the asynchronous call</param>
    /// <returns>A Task indicating status of saving the file</returns>
    public Task SaveAsync(CancellationToken cancellationToken);
    /// <summary>
    /// Random access to the file to avoid reading the entire file into memory.
    ///
    /// When <see cref="rowOffset"/> is 0, and <see cref="characterOffset"/> is 0 that is
    /// the start of the file regardless of whether the file has a BOM preamble
    /// </summary>
    /// <param name="rowOffset">The row index (zero based) to start reading from</param>
    /// <param name="characterOffset">The character index (zero based) to start reading from</param>
    /// <returns>The content read from the file</returns>
    public string Read(long rowOffset, long characterOffset);
    /// <summary>
    /// Random access to the file to avoid reading the entire file into memory.
    ///
    /// When <see cref="rowOffset"/> is 0, and <see cref="characterOffset"/> is 0 that is
    /// the start of the file regardless of whether the file has a BOM preamble
    /// </summary>
    /// <param name="rowOffset">The row index (zero based) to start reading from</param>
    /// <param name="characterOffset">The character index (zero based) to start reading from</param>
    /// <param name="cancellationToken">Relays the cancellation of the asynchronous call</param>
    /// <returns>The content read from the file</returns>
    public Task<string> ReadAsync(long rowOffset, long characterOffset, CancellationToken cancellationToken);
    /// <summary>
    /// Disposes of any unmanaged resources.
    /// </summary>
    public void Dispose();
}