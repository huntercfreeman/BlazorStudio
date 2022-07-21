using BlazorStudio.ClassLib.FileSystem.Files.Interfaces.Files;
using BlazorStudio.ClassLib.FileSystem.Interfaces;

namespace BlazorStudio.ClassLib.FileSystem.Files;

public abstract class FileModelBase : IFileModel
{
    protected FileModelBase(IFileDescriptor? fileDescriptor, 
        IAbsoluteFilePath absoluteFilePath)
    {
        AbsoluteFilePath = absoluteFilePath;
    }

    public IFileDescriptor? FileDescriptor { get; protected set; }
    public IAbsoluteFilePath AbsoluteFilePath { get; }
}