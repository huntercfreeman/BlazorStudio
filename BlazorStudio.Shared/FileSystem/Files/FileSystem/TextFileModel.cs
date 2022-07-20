using BlazorStudio.Shared.FileSystem.Files.Interfaces.Files.FileSystem;
using BlazorStudio.Shared.FileSystem.Interfaces;

namespace BlazorStudio.Shared.FileSystem.Files.FileSystem;

public class TextFileModel : FileModelBase, ITextFileModel
{
    public TextFileModel(IFileDescriptor? fileDescriptor, IAbsoluteFilePath absoluteFilePath)
        : base(fileDescriptor, absoluteFilePath)
    {
    }
}