using BlazorStudio.ClassLib.FileSystem.Interfaces;

namespace BlazorStudio.ClassLib.FileSystem.Classes.Local;

public class LocalFileSystemProvider : IFileSystemProvider
{
    public LocalFileSystemProvider()
    {
        File = new LocalFileHandler();
        Directory = new LocalDirectoryHandler();
    }
    
    public IFileHandler File { get; }
    public IDirectoryHandler Directory { get; }
}