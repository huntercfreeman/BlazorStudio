using BlazorStudio.Shared.FileSystem.Files.Interfaces.Files.FileSystem;
using BlazorStudio.Shared.FileSystem.Interfaces;

namespace BlazorStudio.Shared.FileSystem.Files.FileSystem;

public class UnrecognizedFileModel : FileModelBase, IUnrecognizedFileModel
{
    public UnrecognizedFileModel(IFileDescriptor? fileDescriptor, IAbsoluteFilePath absoluteFilePath)
        : base(fileDescriptor, absoluteFilePath)
    {
    }
}