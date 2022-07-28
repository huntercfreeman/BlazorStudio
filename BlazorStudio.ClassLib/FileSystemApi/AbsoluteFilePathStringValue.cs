using BlazorStudio.ClassLib.FileSystem.Interfaces;

namespace BlazorStudio.ClassLib.FileSystemApi;

public record AbsoluteFilePathStringValue(string AbsoluteFilePathString)
{
    public AbsoluteFilePathStringValue(IAbsoluteFilePath absoluteFilePath) 
        : this(absoluteFilePath.GetAbsoluteFilePathString())
    {
        
    }
}