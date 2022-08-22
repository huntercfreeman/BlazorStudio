using System.Collections.Immutable;
using System.IO.MemoryMappedFiles;
using System.Text;
using BlazorStudio.ClassLib.FileSystem.Interfaces;

namespace BlazorStudio.ClassLib.FileSystemApi.MemoryMapped;

public partial class FileSystemProvider : IFileSystemProvider
{
    private class FileHandleTokenized : IFileHandle
    {
        private readonly Action<IFileHandle> _onDisposeAction;

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
        private FileStream? _fileStream;

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
        public FileHandleKey FileHandleKey { get; } = FileHandleKey.NewFileHandleKey();
        public FileHandleKind FileHandleKind => FileHandleKind.Tokenized;
        public IAbsoluteFilePath AbsoluteFilePath { get; }

        public FileHandleReadRequest? MostRecentReadRequest { get; private set; }

        public FileHandleTokenized(IAbsoluteFilePath absoluteFilePath, Action<IFileHandle> onDisposeAction)
        {
            _onDisposeAction = onDisposeAction;
            AbsoluteFilePath = absoluteFilePath;
        }
        
        public async Task InitializeAsync(CancellationToken cancellationToken)
        {
            if (AbsoluteFilePath.IsDirectory)
                throw new ApplicationException($"{nameof(FileHandleMemoryMapped)} does not support directories.");
        }

        public Task SaveAsync(string content, CancellationToken cancellationToken)
        {
            Dispose();

            File.WriteAllText(AbsoluteFilePath.GetAbsoluteFilePathString(), content);

            //var viewStream = _memoryMappedFile.CreateViewStream();
            //viewStream..Write(data, 0, data.Length); // write hello world

            return Task.CompletedTask;
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

                if (MostRecentReadRequest is not null)
                {
                    throw new ApplicationException("Tokenized FileHandle was already read");
                }
                
                MostRecentReadRequest = readRequest;
                
                var rows = new List<string>();

                var path = AbsoluteFilePath.GetAbsoluteFilePathString();

                var characterCounter = 0;
                var rowCharacterCount = 0;

                using (StreamReader streamReader = new StreamReader(path, true))
                {
                    // Must do a 'read' to get current encoding
                    _ = streamReader.Peek();
                    Encoding = streamReader.CurrentEncoding;

                    var rowBuilder = new StringBuilder();

                    void AddToRows()
                    {
                        _physicalCharacterIndexMarkerForStartOfARow.Add(characterCounter);
                        _virtualCharacterIndexMarkerForStartOfARow.Add(characterCounter);

                        if (rowCharacterCount > PhysicalCharacterLengthOfLongestRow)
                        {
                            PhysicalCharacterLengthOfLongestRow = rowCharacterCount;
                        }

                        rowCharacterCount = 0;

                        rows.Add(rowBuilder.ToString());
                        rowBuilder.Clear();
                    }
                    
                    while (streamReader.Peek() != -1)
                    {
                        var currentCharacter = (char)streamReader.Read();

                        characterCounter++;
                        rowCharacterCount++;
                        rowBuilder.Append(currentCharacter);

                        if (currentCharacter == '\n')
                        {
                            AddToRows();
                        }
                    }
                    
                    if (rowBuilder.Length > 0)
                    {
                        AddToRows();
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
                
                return rows;
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

            if (_fileStream is not null)
            {
                _fileStream.Dispose();
            }

            _onDisposeAction.Invoke(this);
        }
    }
}