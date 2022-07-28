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
        /// <summary>
        /// Index using the Row index and this returns the
        /// starting position of that row within the text file.
        /// </summary>
        private readonly List<long> _characterIndexMarkerForStartOfARow = new()
        {
            0 // Start of document
        };
        
        public ImmutableArray<long> CharacterIndexMarkerForStartOfARow => 
            _characterIndexMarkerForStartOfARow.ToImmutableArray();

        private int _exclusiveEndOfFileCharacterIndex;
        private MemoryMappedFile? _memoryMappedFile;

        // Replace first 2 characters with '~$' to ensure the maximum file path
        // length file system constraint is not hit
        private string MapFileIdentifier =>
            new string(new [] { '~', '$' }
                .Union(AbsoluteFilePath.FilenameWithExtension.Skip(2))
                .ToArray());
        
        public Encoding Encoding { get; private set; }
        public long CharacterLengthOfLongestRow { get; private set; }
        public int RowCount => _characterIndexMarkerForStartOfARow.Count;
        public int ExclusiveEndOfFileCharacterIndex => _exclusiveEndOfFileCharacterIndex;
        public int PreambleLength { get; private set; }
        /// <summary>
        /// TODO: Calculate this instead of assuming each character is 1 byte 
        /// </summary>
        public int BytesPerEncodedCharacter { get; private set; } = 1;

        public FileHandle(IAbsoluteFilePath absoluteFilePath, Action<FileHandle> onDisposeAction)
        {
            _onDisposeAction = onDisposeAction;
            AbsoluteFilePath = absoluteFilePath;
        }

        public void Initialize()
        {
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
                        _characterIndexMarkerForStartOfARow.Add(characterCounter);

                        if (rowCharacterCount > CharacterLengthOfLongestRow)
                        {
                            CharacterLengthOfLongestRow = rowCharacterCount;
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

            _exclusiveEndOfFileCharacterIndex = characterCounter;

            var mapFilename = MapFileIdentifier;
            
            try
            {
                _memoryMappedFile = MemoryMappedFile
                    .CreateFromFile(AbsoluteFilePath.GetAbsoluteFilePathString(),
                        FileMode.Open,
                        mapFilename);
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
        
        public async Task InitializeAsync(CancellationToken cancellationToken)
        {
            // TODO: If it would be useful make an Async version but it might be fine to not have an async initializer?
            Initialize();
        }

        public FileHandleKey FileHandleKey { get; } = FileHandleKey.NewFileHandleKey();
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
            _onDisposeAction.Invoke(this);
        }
    }
}