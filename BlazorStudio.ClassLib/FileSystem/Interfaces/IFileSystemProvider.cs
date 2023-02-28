namespace BlazorStudio.ClassLib.FileSystem.Interfaces;

public interface IFileSystemProvider
{
    public IFileHandler File { get; }
    public IDirectoryHandler Directory { get; }
}