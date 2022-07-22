using System.Collections.Immutable;
using System.IO.MemoryMappedFiles;
using System.Text;
using BlazorStudio.ClassLib.FileSystem.Interfaces;

namespace BlazorStudio.ClassLib.FileSystem.Classes;

public static class FileCoordinateGridFactory
{
    public static async Task<IFileCoordinateGrid> ConstructFileCoordinateGridAsync(IAbsoluteFilePath absoluteFilePath)
    {
        var fileCoordinateGrid = new FileCoordinateGrid(absoluteFilePath);

        await fileCoordinateGrid.InitializeAsync();

        return fileCoordinateGrid;
    }
    
    public static IFileCoordinateGrid ConstructFileCoordinateGrid(IAbsoluteFilePath absoluteFilePath)
    {
        var fileCoordinateGrid = new FileCoordinateGrid(absoluteFilePath);

        fileCoordinateGrid.Initialize();

        return fileCoordinateGrid;
    }

    private record FileCoordinateGrid(IAbsoluteFilePath AbsoluteFilePath)
        : IFileCoordinateGrid
    {
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

        // Replace first 2 characters with '~$' to ensure the maximum file path
        // length file system constraint is not hit
        private readonly Func<string> _getMapFileIdentifierFunc = () =>
                new string(new [] { '~', '$' }
                    .Union(AbsoluteFilePath.FilenameWithExtension.Skip(2))
                    .ToArray());
            
        private int _exclusiveEndOfFileCharacterIndex;

        public Encoding Encoding { get; private set; }
        public long CharacterLengthOfLongestRow { get; private set; }
        public int RowCount => _characterIndexMarkerForStartOfARow.Count;
        public int ExclusiveEndOfFileCharacterIndex => _exclusiveEndOfFileCharacterIndex;

        public void Initialize()
        {
            if (AbsoluteFilePath.IsDirectory)
                throw new ApplicationException($"{nameof(FileCoordinateGrid)} does not support directories.");

            if (AbsoluteFilePath.Directories is null || !AbsoluteFilePath.Directories.Any())
                throw new ApplicationException($"{nameof(AbsoluteFilePath)}.{nameof(AbsoluteFilePath.Directories)} was null or empty");

            var parentDirectory = AbsoluteFilePath.Directories.Last();

            if (parentDirectory.FilePathType != FilePathType.AbsoluteFilePath)
                throw new ApplicationException(
                    $"{nameof(FilePathType)}: '{parentDirectory.FilePathType}' is not currently supported.");

            string path = AbsoluteFilePath.GetAbsoluteFilePathString();

            int characterCounter = 0;
            int? bytesPerEncodedCharacter = null;

            using (StreamReader streamReader = new StreamReader(path, true))
            {
                // Must do a 'read' to get current encoding
                _ = streamReader.Peek();
                Encoding = streamReader.CurrentEncoding;

                var previousCharacter = '\0';

                while (streamReader.Peek() != -1)
                {
                    var currentCharacter = (char)streamReader.Read();

                    characterCounter++;

                    if (currentCharacter == '\n')
                    {
                        // TODO: This commented code seems it would be more accurate way of calculating bytesPerEncodedCharacter and I should revisit this.
                        //if (bytesPerEncodedCharacter is null)
                        //{
                        //    byte[] discardGetBytes = new byte[1];
                        //    char[] arrayForGetBytes = new char[] { currentCharacter };
                        //    var bytesConsumed = Encoding.GetBytes(arrayForGetBytes, 0, 1);

                        //    var intRepresentation = Convert.ToInt32(bytesConsumed.First());

                        //    var vav = 2;
                        //}

                        _characterIndexMarkerForStartOfARow.Add(characterCounter);

                        if (characterCounter > CharacterLengthOfLongestRow)
                        {
                            CharacterLengthOfLongestRow = characterCounter;
                        }
                    }
                    else if (previousCharacter == '\r')
                    {
                        // Count '\r\n' as 1 character
                        characterCounter--;
                    }

                    previousCharacter = currentCharacter;
                }
            }

            _exclusiveEndOfFileCharacterIndex = characterCounter;
        }

        public async Task InitializeAsync()
        {
            Initialize();
        }

        public string Request(FileCoordinateGridRequest fileCoordinateGridRequest)
        {
            long inclusiveStartingCharacter = CharacterIndexMarkerForStartOfARow[fileCoordinateGridRequest.StartingRowIndex];

            var exclusiveEndingRowIndex = fileCoordinateGridRequest.StartingRowIndex + fileCoordinateGridRequest.RowCount;
            
            if (exclusiveEndingRowIndex > CharacterIndexMarkerForStartOfARow.Length - 1)
            {
                exclusiveEndingRowIndex = CharacterIndexMarkerForStartOfARow.Length - 1;
            }

            long exclusiveEndingCharacter;

            if (exclusiveEndingRowIndex == CharacterIndexMarkerForStartOfARow.Length - 1)
            {
                exclusiveEndingCharacter = ExclusiveEndOfFileCharacterIndex;
            }
            else
            {
                exclusiveEndingCharacter = CharacterIndexMarkerForStartOfARow[exclusiveEndingRowIndex];
            }


            long longCharacterLengthOfRequest = exclusiveEndingCharacter - inclusiveStartingCharacter;


            if (longCharacterLengthOfRequest > Int32.MaxValue)
            {
                throw new ApplicationException($"Requested: byte[{longCharacterLengthOfRequest}]," +
                                               $" but the length cannot exceed: byte[{Int32.MaxValue}]");
            }

            int intCharacterLengthOfRequest = (int)longCharacterLengthOfRequest;

            var mapName = _getMapFileIdentifierFunc.Invoke();

            try
            {
                using (var memoryMappedFile = MemoryMappedFile
                        .CreateFromFile(AbsoluteFilePath.GetAbsoluteFilePathString(),
                            FileMode.Open,
                            mapName))
                {
                    return ReadMemoryMappedFile(memoryMappedFile,
                        inclusiveStartingCharacter,
                        intCharacterLengthOfRequest);
                }
            }
            catch (IOException e)
            {
                return e.Message;
            }
        }

        private string ReadMemoryMappedFile(MemoryMappedFile memoryMappedFile, 
            long inclusiveStartingCharacter,
            int intCharacterLengthOfRequest)
        {
            var buffer = new byte[intCharacterLengthOfRequest];

            using (MemoryMappedViewAccessor accessor = memoryMappedFile.CreateViewAccessor(inclusiveStartingCharacter,
                       intCharacterLengthOfRequest))
            {
                accessor.ReadArray(0, buffer, 0, intCharacterLengthOfRequest);
            }

            string memoryMappedFileResult;

            using (StreamReader streamReader = new StreamReader(new MemoryStream(buffer), Encoding))
            {
                var builder = new StringBuilder();

                while (streamReader.Peek() != -1)
                {
                    builder.Append((char)streamReader.Read());
                }

                memoryMappedFileResult = builder.ToString();
            }

            return memoryMappedFileResult;
        }
    }
}

public interface IFileCoordinateGrid
{
    public IAbsoluteFilePath AbsoluteFilePath { get; }
    public Encoding Encoding { get; }
    public long CharacterLengthOfLongestRow { get; }
    public int RowCount { get; }
    public ImmutableArray<long> CharacterIndexMarkerForStartOfARow { get; }

    public string Request(FileCoordinateGridRequest fileCoordinateGridRequest);
}