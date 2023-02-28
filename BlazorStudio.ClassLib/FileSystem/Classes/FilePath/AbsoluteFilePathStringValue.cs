using BlazorStudio.ClassLib.FileSystem.Interfaces;

namespace BlazorStudio.ClassLib.FileSystem.Classes.FilePath;

public record AbsoluteFilePathStringValue(string AbsoluteFilePathString)
{
    public AbsoluteFilePathStringValue(IAbsoluteFilePath absoluteFilePath) 
        : this(absoluteFilePath.GetAbsoluteFilePathString())
    {
        
    }
}