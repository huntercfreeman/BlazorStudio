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

        private readonly string _copyFileIdentifier = "~$bstudio_";
        private int _exclusiveEndOfFileCharacterIndex;

        public IAbsoluteFilePath? CopyAbsoluteFilePath { get; private set; } = null;

        public Encoding Encoding { get; private set; }
        public int RowCount => _characterIndexMarkerForStartOfARow.Count;
        public int ExclusiveEndOfFileCharacterIndex => _exclusiveEndOfFileCharacterIndex;

        public async Task InitializeAsync()
        {
            if (AbsoluteFilePath.IsDirectory)
                throw new ApplicationException($"{nameof(FileCoordinateGrid)} does not support directories.");

            if (AbsoluteFilePath.Directories is null || !AbsoluteFilePath.Directories.Any())
                throw new ApplicationException($"{nameof(AbsoluteFilePath)}.{nameof(AbsoluteFilePath.Directories)} was null or empty");

            var parentDirectory = AbsoluteFilePath.Directories.Last();

            if (parentDirectory.FilePathType != FilePathType.AbsoluteFilePath)
                throw new ApplicationException(
                    $"{nameof(FilePathType)}: '{parentDirectory.FilePathType}' is not currently supported.");

            var containingDirectoryAbsoluteFilePathString =
                ((AbsoluteFilePath)parentDirectory).GetAbsoluteFilePathString();

            //CopyAbsoluteFilePath =
            //    new AbsoluteFilePath(containingDirectoryAbsoluteFilePathString + _copyFileIdentifier + AbsoluteFilePath.FilenameWithExtension,
            //        false);
            //
            CopyAbsoluteFilePath = AbsoluteFilePath;

            string path = CopyAbsoluteFilePath.GetAbsoluteFilePathString();

            //try
            //{
            //    // TODO: Perhaps a way around making a temporary copy of the file is possible
            //    File.Copy(AbsoluteFilePath.GetAbsoluteFilePathString(),
            //        path);
            //}
            //catch (System.IO.IOException)
            //{
            //    // File already exists so use the existing one
            //}

            int characterCounter = 0;
            int? bytesPerEncodedCharacter = null;

            using (StreamReader streamReader = new StreamReader(path, true))
            {
                // Must do a 'read' to get current encoding
                _ = streamReader.Peek();
                Encoding = streamReader.CurrentEncoding;

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
                    }
                }
            }

            _exclusiveEndOfFileCharacterIndex = characterCounter;
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

            // TODO: Virtualize the columns in addition to the rows

            var mapName = _copyFileIdentifier + AbsoluteFilePath.FilenameWithExtension;

            try
            {
                using (var memoryMappedFile = MemoryMappedFile
                        .CreateFromFile(CopyAbsoluteFilePath.GetAbsoluteFilePathString(),
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
                var z = 2;

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

        public void Dispose()
        {
            //if (_memoryMappedFile is not null)
            //    _memoryMappedFile.Dispose();

            //if (CopyAbsoluteFilePath is not null)
            //    File.Delete(CopyAbsoluteFilePath.GetAbsoluteFilePathString());
        }
    }
}

public interface IFileCoordinateGrid
{
    public IAbsoluteFilePath AbsoluteFilePath { get; }
    public Encoding Encoding { get; }
    public int RowCount { get; }
    public IAbsoluteFilePath? CopyAbsoluteFilePath { get; }
    public ImmutableArray<long> CharacterIndexMarkerForStartOfARow { get; }

    public string Request(FileCoordinateGridRequest fileCoordinateGridRequest);
    /// <summary>
    /// Added this Dispose declaration so I can call it when Unit Testing
    /// IDisposable should call Dispose() for you.
    /// </summary>
    public void Dispose();
}