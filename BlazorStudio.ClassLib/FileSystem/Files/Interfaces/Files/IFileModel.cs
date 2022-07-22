using BlazorStudio.ClassLib.FileSystem.Interfaces;

namespace BlazorStudio.ClassLib.FileSystem.Files.Interfaces.Files;

public interface IFileModel
{
    public IFileDescriptor? FileDescriptor { get; }
    public IAbsoluteFilePath AbsoluteFilePath { get; }
}