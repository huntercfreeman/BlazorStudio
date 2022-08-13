using BlazorStudio.ClassLib.FileSystem.Interfaces;

namespace BlazorStudio.ClassLib.FileSystemApi.MemoryMapped;

public record AbsoluteFilePathStringValue(string AbsoluteFilePathString)
{
    public AbsoluteFilePathStringValue(IAbsoluteFilePath absoluteFilePath) 
        : this(absoluteFilePath.GetAbsoluteFilePathString())
    {
        
    }
}