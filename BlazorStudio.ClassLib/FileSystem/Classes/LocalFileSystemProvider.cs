using BlazorStudio.ClassLib.FileSystem.Interfaces;

namespace BlazorStudio.ClassLib.FileSystem.Classes;

public class LocalFileSystemProvider : IFileSystemProvider
{
    public async Task WriteFileAsync(
        IAbsoluteFilePath absoluteFilePath,
        string content,
        bool overwrite,
        bool create,
        CancellationToken cancellationToken = default)
    {
        await File
            .WriteAllTextAsync(
                absoluteFilePath.GetAbsoluteFilePathString(),
                content,
                cancellationToken);
    }

    public async Task<string> ReadFileAsync(
        IAbsoluteFilePath absoluteFilePath,
        CancellationToken cancellationToken = default)
    {
        return await File.ReadAllTextAsync(
            absoluteFilePath.GetAbsoluteFilePathString(),
            cancellationToken);
    }
}