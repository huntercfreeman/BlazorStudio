namespace BlazorStudio.ClassLib.FileSystem.Interfaces;

/// <summary>
/// https://vshaxe.github.io/vscode-extern/vscode/FileSystemProvider.html#:~:text=package%20vscode,integrate%20those%20into%20the%20editor.
/// <br/>--<br/>
/// Writing the same API interface as Visual Studio Code's
/// "FileSystemProvider".
/// </summary>
public interface IFileSystemProvider
{
    public Task WriteFileAsync(IAbsoluteFilePath absoluteFilePath, string content, bool overwrite, bool create, CancellationToken cancellationToken = default);
    public Task<string> ReadFileAsync(IAbsoluteFilePath absoluteFilePath, CancellationToken cancellationToken = default);
    public Task CreateDirectoryAsync(IAbsoluteFilePath absoluteFilePath, CancellationToken cancellationToken = default);
}