using BlazorStudio.Shared.FileSystem.Interfaces;

namespace BlazorStudio.Shared.FileSystem.Classes;

public class FileSystemDrive : IFileSystemDrive
{
    public FileSystemDrive(string driveNameAsIdentifier)
    {
        DriveNameAsIdentifier = driveNameAsIdentifier;
    }

    public string DriveNameAsIdentifier { get; }
    public string DriveNameAsPath => $"{DriveNameAsIdentifier}:{System.IO.Path.DirectorySeparatorChar}";
}