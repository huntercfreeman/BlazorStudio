using BlazorStudio.Shared.FileSystem.Files.Interfaces.Files.DotNet.CSharp;
using BlazorStudio.Shared.FileSystem.Interfaces;

namespace BlazorStudio.Shared.FileSystem.Files.DotNet.CSharp;

public class CSharpFileModel : FileModelBase, ICSharpFileModel
{
    public CSharpFileModel(IFileDescriptor? fileDescriptor, IAbsoluteFilePath absoluteFilePath)
        : base(fileDescriptor, absoluteFilePath)
    {
    }
}