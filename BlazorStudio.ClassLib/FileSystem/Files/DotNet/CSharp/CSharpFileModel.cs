using BlazorStudio.ClassLib.FileSystem.Files.Interfaces.Files.DotNet.CSharp;
using BlazorStudio.ClassLib.FileSystem.Interfaces;

namespace BlazorStudio.ClassLib.FileSystem.Files.DotNet.CSharp;

public class CSharpFileModel : FileModelBase, ICSharpFileModel
{
    public CSharpFileModel(IFileDescriptor? fileDescriptor, IAbsoluteFilePath absoluteFilePath)
        : base(fileDescriptor, absoluteFilePath)
    {
    }
}