namespace BlazorStudio.Shared.FileSystem.Interfaces;

public interface IRelativeFilePath : IFilePath
{
    public string GetRelativeFilePathString();
}