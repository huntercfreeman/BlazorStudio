using System.Collections.Immutable;
using System.Text;
using BlazorStudio.ClassLib.FileSystem.Interfaces;

namespace BlazorStudio.ClassLib.FileSystemApi;

public class EditBuilder
{
    
}

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
    public Encoding Encoding { get; }
    public long CharacterLengthOfLongestRow { get; }
    public int RowCount { get; }
    public int ExclusiveEndOfFileCharacterIndex { get; }
    public int PreambleLength { get; }
    /// <summary>
    /// TODO: Calculate this instead of assuming each character is 1 byte 
    /// </summary>
    public int BytesPerEncodedCharacter { get; }
    public ImmutableArray<long> CharacterIndexMarkerForStartOfARow { get; }
    
    /// <summary>
    /// Random access to the file to avoid reading the entire file into memory.
    /// 
    /// When <see cref="rowIndexOffset"/> is 0, and <see cref="characterIndexOffset"/> is 0 that is
    /// the start of the file regardless of whether the file has a BOM preamble
    /// </summary>
    /// <param name="rowIndexOffset">The row index (zero based and inclusive) to start reading from</param>
    /// <param name="characterIndexOffset">The character index (zero based and inclusive) to start reading from</param>
    /// <param name="rowCount">The amount of rows to read.</param>
    /// <param name="characterCount">The amount of characters to read</param>
    /// <param name="cancellationToken">Relays the cancellation of the asynchronous call</param>
    /// <returns>The content read from the file as a List where each entry is a separate row</returns>
    public List<string> Read(int rowIndexOffset, int characterIndexOffset, int rowCount, int characterCount, 
        CancellationToken cancellationToken);
    /// <summary>
    /// Random access to the file to avoid reading the entire file into memory.
    ///
    /// When <see cref="rowIndexOffset"/> is 0, and <see cref="characterIndexOffset"/> is 0 that is
    /// the start of the file regardless of whether the file has a BOM preamble
    /// </summary>
    /// <param name="rowIndexOffset">The row index (zero based) to start reading from</param>
    /// <param name="characterIndexOffset">The character index (zero based) to start reading from</param>
    /// <param name="rowCount">The amount of rows to read.</param>
    /// <param name="characterCount">The amount of characters to read</param>
    /// <param name="cancellationToken">Relays the cancellation of the asynchronous call</param>
    /// <returns>The content read from the file</returns>
    public Task<List<string>> ReadAsync(int rowIndexOffset, int characterIndexOffset, int rowCount, int characterCount, 
        CancellationToken cancellationToken);
    
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
    /// Make an edit that will be seen when reading from the same FileHandle. However,
    /// this will not edit the file on disk. One must <see cref="Save"/> after editing to persist changes.
    /// </summary>
    /// <returns>An EditBuilder to fluently chain multiple edits together.</returns>
    public EditBuilder Edit();
    /// <summary>
    /// Make an edit that will be seen when reading from the same FileHandle. However,
    /// this will not edit the file on disk. One must <see cref="Save"/> after editing to persist changes.
    /// </summary>
    /// <param name="cancellationToken">Relays the cancellation of the asynchronous call</param>
    /// <returns>A Task that wraps an EditBuilder to fluently chain multiple edits together.</returns>
    public Task<EditBuilder> EditAsync(CancellationToken cancellationToken);
    
    /// <summary>
    /// Disposes of any unmanaged resources.
    /// </summary>
    public void Dispose();
}