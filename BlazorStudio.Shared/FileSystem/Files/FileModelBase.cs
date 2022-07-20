using BlazorStudio.Shared.FileSystem.Files.Interfaces.Files;
using BlazorStudio.Shared.FileSystem.Interfaces;

namespace BlazorStudio.Shared.FileSystem.Files;

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