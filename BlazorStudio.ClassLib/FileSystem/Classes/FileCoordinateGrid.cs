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
        : IFileCoordinateGrid, IDisposable
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
        private MemoryMappedFile _memoryMappedFile;

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
        }

        public async Task InitializeAsync()
        {
            Initialize();
        }

        public List<string> Request(FileCoordinateGridRequest fileCoordinateGridRequest)
        {
            var rowBuilders = new List<StringBuilder>();

            var availableRowCount = Math.Min(fileCoordinateGridRequest.RowCount, CharacterIndexMarkerForStartOfARow.Length);

            for (int i = fileCoordinateGridRequest.StartingRowIndex; i < availableRowCount; i++)
            {
                var builder = new StringBuilder();
                rowBuilders.Add(builder);

                long inclusiveStartingCharacterIndex = CharacterIndexMarkerForStartOfARow[i] +
                                                  fileCoordinateGridRequest.StartingCharacterIndex;

                var exclusiveEndingCharacterIndex = inclusiveStartingCharacterIndex + fileCoordinateGridRequest.CharacterCount;

                // Ensure within bounds of file
                exclusiveEndingCharacterIndex = exclusiveEndingCharacterIndex > ExclusiveEndOfFileCharacterIndex
                    ? ExclusiveEndOfFileCharacterIndex
                    : exclusiveEndingCharacterIndex;

                // Ensure within bounds of row
                if (i < CharacterIndexMarkerForStartOfARow.Length - 1)
                {
                    long startOfNextRowCharacterIndex = CharacterIndexMarkerForStartOfARow[i + 1];

                    exclusiveEndingCharacterIndex = exclusiveEndingCharacterIndex > startOfNextRowCharacterIndex
                        ? startOfNextRowCharacterIndex
                        : exclusiveEndingCharacterIndex;
                }

                long longCharacterLengthOfRequest = exclusiveEndingCharacterIndex - inclusiveStartingCharacterIndex;


                if (longCharacterLengthOfRequest > Int32.MaxValue)
                {
                    throw new ApplicationException($"Requested: byte[{longCharacterLengthOfRequest}]," +
                                                   $" but the length cannot exceed: byte[{Int32.MaxValue}]");
                }

                int intCharacterLengthOfRequest = (int)longCharacterLengthOfRequest;

                var buffer = new byte[intCharacterLengthOfRequest];

                using (MemoryMappedViewAccessor accessor = _memoryMappedFile.CreateViewAccessor(inclusiveStartingCharacterIndex,
                           intCharacterLengthOfRequest))
                {
                    accessor.ReadArray(0, buffer, 0, intCharacterLengthOfRequest);
                }

                using (StreamReader streamReader = new StreamReader(new MemoryStream(buffer), Encoding))
                {
                    while (streamReader.Peek() != -1)
                    {
                        builder.Append((char)streamReader.Read());
                    }
                }
            }

            return rowBuilders
                .Select(x => x.ToString())
                .ToList();
        }

        public void Dispose()
        {
            _memoryMappedFile.Dispose();
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

    public List<string> Request(FileCoordinateGridRequest fileCoordinateGridRequest);
    /// <summary>
    /// Needed when Unit Testing as it won't auto dispose
    /// </summary>
    public void Dispose();
}