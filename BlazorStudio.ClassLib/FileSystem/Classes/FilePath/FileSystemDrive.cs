using BlazorStudio.ClassLib.FileSystem.Interfaces;

namespace BlazorStudio.ClassLib.FileSystem.Classes.FilePath;

public class FileSystemDrive : IFileSystemDrive
{
    public FileSystemDrive(
        string driveNameAsIdentifier,
        IEnvironmentProvider environmentProvider)
    {
        DriveNameAsIdentifier = driveNameAsIdentifier;
        EnvironmentProvider = environmentProvider;
    }

    public string DriveNameAsIdentifier { get; }
    public IEnvironmentProvider EnvironmentProvider { get; }
    public string DriveNameAsPath => $"{DriveNameAsIdentifier}:{EnvironmentProvider.DirectorySeparatorChar}";
}