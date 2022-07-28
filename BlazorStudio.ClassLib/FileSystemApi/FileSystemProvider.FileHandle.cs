using BlazorStudio.ClassLib.FileSystem.Interfaces;

namespace BlazorStudio.ClassLib.FileSystemApi;

public partial class FileSystemProvider : IFileSystemProvider
{
    private class FileHandle : IFileHandle
    {
        private readonly Action<FileHandle> _onDisposeAction;

        public FileHandle(IAbsoluteFilePath absoluteFilePath, Action<FileHandle> onDisposeAction)
        {
            _onDisposeAction = onDisposeAction;
            AbsoluteFilePath = absoluteFilePath;
        }

        public void Initialize()
        {
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

            try
            {
                _memoryMappedFile = MemoryMappedFile
                    .CreateFromFile(AbsoluteFilePath.GetAbsoluteFilePathString(),
                        FileMode.Open,
                        _getMapFileIdentifierFunc.Invoke());
            }
            catch (IOException e)
            {
                File.Delete(_getMapFileIdentifierFunc.Invoke());

                _memoryMappedFile = MemoryMappedFile
                    .CreateFromFile(AbsoluteFilePath.GetAbsoluteFilePathString(),
                        FileMode.Open,
                        _getMapFileIdentifierFunc.Invoke());
            }
            catch (System.ArgumentException e)
            {
                // The file exists but is empty.
            }
        }
        
        public async Task InitializeAsync(CancellationToken cancellationToken)
        {
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

            try
            {
                _memoryMappedFile = MemoryMappedFile
                    .CreateFromFile(AbsoluteFilePath.GetAbsoluteFilePathString(),
                        FileMode.Open,
                        _getMapFileIdentifierFunc.Invoke());
            }
            catch (IOException e)
            {
                File.Delete(_getMapFileIdentifierFunc.Invoke());

                _memoryMappedFile = MemoryMappedFile
                    .CreateFromFile(AbsoluteFilePath.GetAbsoluteFilePathString(),
                        FileMode.Open,
                        _getMapFileIdentifierFunc.Invoke());
            }
            catch (System.ArgumentException e)
            {
                // The file exists but is empty.
            }
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