using BlazorStudio.ClassLib.FileSystem.Interfaces;

namespace BlazorStudio.ClassLib.FileSystem.Classes;

public class FilePersister : IFilePersister
{
    public void Write(IAbsoluteFilePath writeLocationAbsoluteFilePath, string? contents)
    {
        System.IO.File.WriteAllText(writeLocationAbsoluteFilePath.GetAbsoluteFilePathString(), contents);
    }

    public async Task WriteAsync(IAbsoluteFilePath writeLocationAbsoluteFilePath, string? contents)
    {
        await System.IO.File.WriteAllTextAsync(writeLocationAbsoluteFilePath.GetAbsoluteFilePathString(), contents);
    }
}