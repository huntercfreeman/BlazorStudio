namespace BlazorStudio.ClassLib.FileSystem.Interfaces;

public interface IFilePersister
{
    public void Write(IAbsoluteFilePath writeLocationAbsoluteFilePath, string? contents);
    public Task WriteAsync(IAbsoluteFilePath writeLocationAbsoluteFilePath, string? contents);
}