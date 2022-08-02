using System.Collections.Immutable;
using System.IO.MemoryMappedFiles;
using System.Text;
using BlazorStudio.ClassLib.FileSystem.Interfaces;

namespace BlazorStudio.ClassLib.FileSystemApi;

public partial class FileSystemProvider : IFileSystemProvider
{
    private class FileHandle : IFileHandle
    {
        private readonly Action<FileHandle> _onDisposeAction;

        private readonly SemaphoreSlim _readSemaphoreSlim = new(1, 1);

        /// <summary>
        /// Index using the Row index and this returns the
        /// starting position of that row within the text file.
        /// </summary>
        private readonly List<long> _physicalCharacterIndexMarkerForStartOfARow = new()
        {
            0 // Start of document
        };
        /// <summary>
        /// Index using the Row index and this returns the
        /// starting position of that row within the text file.
        /// </summary>
        private readonly List<long> _virtualCharacterIndexMarkerForStartOfARow = new()
        {
            0 // Start of document
        };
        
        private MemoryMappedFile? _memoryMappedFile;

        public EditBuilder Edit { get; } = EditBuilder.Build();
        
        // Replace first 2 characters with '~$' to ensure the maximum file path
        // length file system constraint is not hit
        private string MapFileIdentifier =>
            new string(new [] { '~', '$' }
                .Union(AbsoluteFilePath.FilenameWithExtension.Skip(2))
                .ToArray());
        
        public Encoding Encoding { get; private set; }
        
        public long PhysicalCharacterLengthOfLongestRow { get; private set; }
        public int PhysicalExclusiveEndOfFileCharacterIndex { get; private set; }
        public int PhysicalRowCount => _physicalCharacterIndexMarkerForStartOfARow.Count;
        public ImmutableArray<long> PhysicalCharacterIndexMarkerForStartOfARow => 
            _physicalCharacterIndexMarkerForStartOfARow.ToImmutableArray();
        
        public long VirtualCharacterLengthOfLongestRow { get; private set; }
        public int VirtualExclusiveEndOfFileCharacterIndex { get; private set; }
        public int VirtualRowCount => _virtualCharacterIndexMarkerForStartOfARow.Count;
        public List<long> VirtualCharacterIndexMarkerForStartOfARow => 
            _virtualCharacterIndexMarkerForStartOfARow;
        
        public int PreambleLength { get; private set; }
        /// <summary>
        /// TODO: Calculate this instead of assuming each character is 1 byte 
        /// </summary>
        public int BytesPerEncodedCharacter { get; private set; } = 1;

        public FileHandleReadRequest MostRecentReadRequest { get; private set; }

        public FileHandle(IAbsoluteFilePath absoluteFilePath, Action<FileHandle> onDisposeAction)
        {
            _onDisposeAction = onDisposeAction;
            AbsoluteFilePath = absoluteFilePath;
        }
        
        public async Task InitializeAsync(CancellationToken cancellationToken)
        {
#if RELEASE
            return;
#endif

            if (AbsoluteFilePath.IsDirectory)
                throw new ApplicationException($"{nameof(FileHandle)} does not support directories.");

            var path = AbsoluteFilePath.GetAbsoluteFilePathString();

            var characterCounter = 0;
            var rowCharacterCount = 0;

            using (StreamReader streamReader = new StreamReader(path, true))
            {
                // Must do a 'read' to get current encoding
                _ = streamReader.Peek();
                Encoding = streamReader.CurrentEncoding;

                while (streamReader.Peek() != -1)
                {
                    var currentCharacter = (char)streamReader.Read();

                    characterCounter++;
                    rowCharacterCount++;

                    if (currentCharacter == '\n')
                    {
                        _physicalCharacterIndexMarkerForStartOfARow.Add(characterCounter);
                        _virtualCharacterIndexMarkerForStartOfARow.Add(characterCounter);

                        if (rowCharacterCount > PhysicalCharacterLengthOfLongestRow)
                        {
                            PhysicalCharacterLengthOfLongestRow = rowCharacterCount;
                        }

                        rowCharacterCount = 0;
                    }
                }
            }

            using (FileStream fs = new FileStream(path, FileMode.Open))
            {
                byte[] bits = new byte[3];
                var readAmount = fs.Read(bits, 0, 3);

                if (readAmount == 3)
                {
                    // UTF8 byte order mark is: 0xEF,0xBB,0xBF
                    if (bits[0] == Encoding.Preamble[0] && bits[1] == Encoding.Preamble[1] &&
                        bits[2] == Encoding.Preamble[2])
                    {
                        PreambleLength = 3;
                    }
                }
            }

            PhysicalExclusiveEndOfFileCharacterIndex = characterCounter;

            var mapFilename = MapFileIdentifier;

            try
            {
                if (PhysicalCharacterLengthOfLongestRow != 0)
                {
                    _memoryMappedFile = MemoryMappedFile
                        .CreateFromFile(AbsoluteFilePath.GetAbsoluteFilePathString(),
                            FileMode.Open,
                            mapFilename);

                }
            }
            catch (IOException e)
            {
                File.Delete(mapFilename);

                _memoryMappedFile = MemoryMappedFile
                    .CreateFromFile(AbsoluteFilePath.GetAbsoluteFilePathString(),
                        FileMode.Open,
                        mapFilename);
            }
            catch (System.ArgumentException e)
            {
                // The file exists but is empty.
            }
        }

        public FileHandleKey FileHandleKey { get; } = FileHandleKey.NewFileHandleKey();
        public IAbsoluteFilePath AbsoluteFilePath { get; }

        public Task SaveAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
        
        public async Task<List<string>?> ReadAsync(FileHandleReadRequest readRequest)
        {
            try
            {
                await _readSemaphoreSlim.WaitAsync();

                if (readRequest.CancellationToken.IsCancellationRequested)
                {
                    return null;
                }

                MostRecentReadRequest = readRequest;

                var rows = new List<string>();

#if RELEASE
            return (await Edit.ApplyEditsAsync(readRequest, rows, _virtualCharacterIndexMarkerForStartOfARow, CancellationToken.None))
                .ContentRows;
#endif

                if (_memoryMappedFile is not null)
                {
                    var availableRowCount = Math.Max(
                        _physicalCharacterIndexMarkerForStartOfARow.Count - readRequest.RowIndexOffset,
                        0);

                    var toReadRowCount = Math.Min(readRequest.RowCount, availableRowCount);

                    var rowIndex = readRequest.RowIndexOffset;

                    var rowsRead = 0;

                    for (;
                         rowsRead < toReadRowCount;
                         rowsRead++, rowIndex++)
                    {
                        if (readRequest.CancellationToken.IsCancellationRequested)
                            return rows;

                        long inclusiveStartingCharacterIndex = _physicalCharacterIndexMarkerForStartOfARow[rowIndex] +
                                                               readRequest.CharacterIndexOffset;

                        var exclusiveEndingCharacterIndex =
                            inclusiveStartingCharacterIndex + readRequest.CharacterCount;

                        // Ensure within bounds of file
                        exclusiveEndingCharacterIndex = exclusiveEndingCharacterIndex > PhysicalExclusiveEndOfFileCharacterIndex
                            ? PhysicalExclusiveEndOfFileCharacterIndex
                            : exclusiveEndingCharacterIndex;

                        // Ensure within bounds of row
                        if (rowIndex < _physicalCharacterIndexMarkerForStartOfARow.Count - 1)
                        {
                            long startOfNextRowCharacterIndex = _physicalCharacterIndexMarkerForStartOfARow[rowIndex + 1];

                            exclusiveEndingCharacterIndex = exclusiveEndingCharacterIndex > startOfNextRowCharacterIndex
                                ? startOfNextRowCharacterIndex
                                : exclusiveEndingCharacterIndex;
                        }

                        exclusiveEndingCharacterIndex = Math.Min(PhysicalExclusiveEndOfFileCharacterIndex, exclusiveEndingCharacterIndex);

                        long longCharacterLengthOfRequest = exclusiveEndingCharacterIndex - inclusiveStartingCharacterIndex;

                        if (longCharacterLengthOfRequest <= 0)
                        {
                            rows.Add(string.Empty);
                            continue;
                        }

                        if (longCharacterLengthOfRequest > Int32.MaxValue)
                        {
                            throw new ApplicationException($"Requested: byte[{longCharacterLengthOfRequest}]," +
                                                           $" but the length cannot exceed: byte[{Int32.MaxValue}]");
                        }

                        int intCharacterLengthOfRequest = (int)longCharacterLengthOfRequest;

                        using var stream = _memoryMappedFile
                            .CreateViewStream(PreambleLength + inclusiveStartingCharacterIndex,
                                intCharacterLengthOfRequest,
                                MemoryMappedFileAccess.Read);

                        using var reader = new StreamReader(stream, Encoding);

                        rows.Add(reader.ReadToEnd());
                    }
                }
                else
                {
                    rows.Add(string.Empty);
                }

                // TODO: Add the displacement to the Virtual values
                VirtualCharacterLengthOfLongestRow = PhysicalCharacterLengthOfLongestRow;

                var editedResult = await Edit.ApplyEditsAsync(readRequest, 
                    rows, 
                    _virtualCharacterIndexMarkerForStartOfARow);

                return editedResult.ContentRows;
            }
            finally
            {
                _readSemaphoreSlim.Release();
            }
        }

        public void Dispose()
        {
            if (_memoryMappedFile is not null)
            {
                _memoryMappedFile.Dispose();
            }
            
            _onDisposeAction.Invoke(this);
        }
    }
}