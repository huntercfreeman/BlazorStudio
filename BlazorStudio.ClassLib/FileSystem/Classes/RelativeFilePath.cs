using System.Text;
using BlazorStudio.ClassLib.FileSystem.Interfaces;

namespace BlazorStudio.ClassLib.FileSystem.Classes;

public class RelativeFilePath : IRelativeFilePath
{
    private readonly StringBuilder _tokenBuilder = new();
    private readonly int _position;

    public RelativeFilePath(List<IFilePath> directories,
        string fileNameNoExtension,
        string extensionNoPeriod,
        bool isDirectory)
    {
        Directories = directories;
        FileNameNoExtension = fileNameNoExtension;
        ExtensionNoPeriod = extensionNoPeriod;
        IsDirectory = isDirectory;
    }

    public RelativeFilePath(string relativeFilePathString, bool isDirectory)
    {
        // TODO: Handle ../../myFile.c

        if (relativeFilePathString.StartsWith('.'))
        {
            while (_position < relativeFilePathString.Length)
            {
                var currentCharacter = relativeFilePathString[_position++];

                /*
                 * System.IO.Path.DirectorySeparatorChar is not a constant character
                 * As a result this is an if statement instead of a switch statement
                 */
                if (currentCharacter == Path.DirectorySeparatorChar ||
                    currentCharacter == Path.AltDirectorySeparatorChar)
                    break;
            }
        }

        IsDirectory = isDirectory;

        while (_position < relativeFilePathString.Length)
        {
            var currentCharacter = relativeFilePathString[_position++];

            /*
             * System.IO.Path.DirectorySeparatorChar is not a constant character
             * As a result this is an if statement instead of a switch statement
             */
            if (currentCharacter == Path.DirectorySeparatorChar ||
                currentCharacter == Path.AltDirectorySeparatorChar)
                ConsumeTokenAsDirectory();
            else
                _tokenBuilder.Append(currentCharacter);
        }

        var fileNameWithExtension = _tokenBuilder.ToString();

        var splitFileName = fileNameWithExtension.Split('.');

        if (splitFileName.Length == 2)
        {
            FileNameNoExtension = splitFileName[0];
            ExtensionNoPeriod = splitFileName[1];
        }
        else if (splitFileName.Length == 1)
        {
            FileNameNoExtension = splitFileName[0];
            ExtensionNoPeriod = string.Empty;
        }
        else
        {
            StringBuilder fileNameBuilder = new();

            foreach (var split in splitFileName.SkipLast(1)) fileNameBuilder.Append($"{split}.");

            fileNameBuilder.Remove(fileNameBuilder.Length - 1, 1);

            FileNameNoExtension = fileNameBuilder.ToString();
            ExtensionNoPeriod = splitFileName.Last();
        }
    }

    public string GetRelativeFilePathString()
    {
        StringBuilder absoluteFilePathStringBuilder = new();

        if (Directories.Any()) absoluteFilePathStringBuilder.Append(Directories.Select(d => d.FilenameWithExtension));

        absoluteFilePathStringBuilder.Append(FilenameWithExtension);

        return absoluteFilePathStringBuilder.ToString();
    }

    public FilePathType FilePathType { get; } = FilePathType.RelativeFilePath;
    public bool IsDirectory { get; protected set; }
    public List<IFilePath> Directories { get; } = new();
    public string FileNameNoExtension { get; protected set; }
    public string ExtensionNoPeriod { get; protected set; }
    public string FilenameWithExtension => FileNameNoExtension +
                                           (IsDirectory
                                               ? Path.DirectorySeparatorChar.ToString()
                                               : $".{ExtensionNoPeriod}");

    public void ConsumeTokenAsDirectory()
    {
        var directoryFilePath = (IFilePath)new RelativeFilePath(new List<IFilePath>(Directories),
            _tokenBuilder.ToString(),
            Path.DirectorySeparatorChar.ToString(),
            true);

        Directories.Add(directoryFilePath);

        _tokenBuilder.Clear();
    }
}