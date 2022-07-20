using BlazorStudio.Shared.FileSystem.Files.Interfaces.Files.FileSystem;
using BlazorStudio.Shared.FileSystem.Interfaces;

namespace BlazorStudio.Shared.FileSystem.Files.FileSystem;

public class DirectoryFileModel : FileModelBase, IDirectoryFileModel
{
    public DirectoryFileModel(IFileDescriptor? fileDescriptor, IAbsoluteFilePath absoluteFilePath)
        : base(fileDescriptor, absoluteFilePath)
    {
    }
}