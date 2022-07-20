using BlazorStudio.Shared.FileSystem.Interfaces;

namespace BlazorStudio.Shared.FileSystem.Files.Interfaces.Files;

public interface IFileModel
{
    public IFileDescriptor? FileDescriptor { get; }
    public IAbsoluteFilePath AbsoluteFilePath { get; }
}