using BlazorStudio.ClassLib.FileSystem.Files.Interfaces.Files.FileSystem;
using BlazorStudio.ClassLib.FileSystem.Interfaces;

namespace BlazorStudio.ClassLib.FileSystem.Files.FileSystem;

public class TextFileModel : FileModelBase, ITextFileModel
{
    public TextFileModel(IFileDescriptor? fileDescriptor, IAbsoluteFilePath absoluteFilePath)
        : base(fileDescriptor, absoluteFilePath)
    {
    }
}