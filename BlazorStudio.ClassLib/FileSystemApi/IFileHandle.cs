using System.Collections.Immutable;
using System.Text;
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
    /// paths are unambiguous so this is typed accordingly.
    /// </summary>
    public IAbsoluteFilePath AbsoluteFilePath { get; }
    /// <summary>
    /// Make an edit that will be seen when reading from the same FileHandle. However,
    /// this will not edit the file on disk. One must <see cref="Save"/> after editing to persist changes.
    /// </summary>
    public EditBuilder Edit { get; }
    public Encoding Encoding { get; }
    public long PhysicalCharacterLengthOfLongestRow { get; }
    public int PhysicalRowCount { get; }
    public int PhysicalExclusiveEndOfFileCharacterIndex { get; }
    public ImmutableArray<long> PhysicalCharacterIndexMarkerForStartOfARow { get; }
    public long VirtualCharacterLengthOfLongestRow { get; }
    public int VirtualRowCount { get; }
    public int VirtualExclusiveEndOfFileCharacterIndex { get; }
    public List<long> VirtualCharacterIndexMarkerForStartOfARow { get; }
    public int PreambleLength { get; }
    /// <summary>
    /// TODO: Calculate this instead of assuming each character is 1 byte 
    /// </summary>
    public int BytesPerEncodedCharacter { get; }
    public FileHandleReadRequest MostRecentReadRequest { get; }

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
    /// <returns>The content read from the file or null if the request was cancelled</returns>
    public Task<List<string>?> ReadAsync(FileHandleReadRequest readRequest);
    
    /// <summary>
    /// Write out any changes pending in memory to the FileSystem
    /// </summary>
    /// <param name="cancellationToken">Relays the cancellation of the asynchronous call</param>
    /// <returns>A Task indicating status of saving the file</returns>
    public Task SaveAsync(string content, CancellationToken cancellationToken);
}