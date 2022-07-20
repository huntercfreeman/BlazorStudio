using System.Collections.Immutable;
using System.IO.MemoryMappedFiles;
using System.Text;
using BlazorStudio.Shared.FileSystem.Interfaces;

namespace BlazorStudio.Shared.FileSystem.Classes;

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
        private readonly List<long> _byteMarkerForStartOfARow = new()
        {
            0 // Start of document
        };

        private IAbsoluteFilePath? _copyAbsoluteFilePath = null;

        public Encoding Encoding { get; private set; }
        public int RowCount => _byteMarkerForStartOfARow.Count;

        public async Task InitializeAsync()
        {
            if (AbsoluteFilePath.IsDirectory)
                throw new ApplicationException($"{nameof(FileCoordinateGrid)} does not support directories.");

            if (AbsoluteFilePath.Directories?.Any() ?? true)
                throw new ApplicationException($"{nameof(AbsoluteFilePath)}.{nameof(AbsoluteFilePath.Directories)} was null or empty");

            var parentDirectory = AbsoluteFilePath.Directories.Last();

            if (parentDirectory.FilePathType != FilePathType.AbsoluteFilePath)
                throw new ApplicationException(
                    $"{nameof(FilePathType)}: '{parentDirectory.FilePathType}' is not currently supported.");

            var containingDirectoryAbsoluteFilePathString =
                ((AbsoluteFilePath)parentDirectory).GetAbsoluteFilePathString();

            _copyAbsoluteFilePath =
                new AbsoluteFilePath(containingDirectoryAbsoluteFilePathString + AbsoluteFilePath.FilenameWithExtension,
                    false);

            string path = _copyAbsoluteFilePath.GetAbsoluteFilePathString();

            // TODO: Perhaps a way around making a temporary copy of the file is possible
            File.Copy(AbsoluteFilePath.GetAbsoluteFilePathString(),
                path);

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

                        _byteMarkerForStartOfARow.Add(characterCounter);
                    }
                }
            }
        }

        public void Dispose()
        {
            if (_copyAbsoluteFilePath is not null)
                File.Delete(_copyAbsoluteFilePath.GetAbsoluteFilePathString());
        }
    }
}

public interface IFileCoordinateGrid
{
    public IAbsoluteFilePath AbsoluteFilePath { get; }
    public Encoding Encoding { get; }
    public int RowCount { get; }
}