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
    public Task DeleteFileAsync(IAbsoluteFilePath absoluteFilePath, CancellationToken cancellationToken = default);
    public Task DeleteDirectoryAsync(IAbsoluteFilePath absoluteFilePath, bool recursive, CancellationToken cancellationToken = default);
    public bool FileExists(string absoluteFilePathString);
    public void FileCopy(string sourceAbsoluteFilePathString, string destinationAbsoluteFilePathString);
    public void FileMove(string sourceAbsoluteFilePathString, string destinationAbsoluteFilePathString);
    public DateTime FileGetLastWriteTime(string absoluteFilePathString);
    public Task<string> FileReadAllTextAsync(string absoluteFilePathString);
    public Task WriteAllTextAsync(string absoluteFilePathString, string? contents, CancellationToken cancellationToken = default(CancellationToken));
    public bool DirectoryExists(string absoluteFilePathString);
    public void DirectoryMove(string sourceAbsoluteFilePathString, string destinationAbsoluteFilePathString);
    public string[] DirectoryGetDirectories(string absoluteFilePathString);
    public string[] DirectoryGetFiles(string absoluteFilePathString);
    public IEnumerable<string> DirectoryEnumerateFileSystemEntries(string absoluteFilePathString);
}