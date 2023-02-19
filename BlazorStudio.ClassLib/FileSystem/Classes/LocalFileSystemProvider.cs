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

    public Task CreateDirectoryAsync(IAbsoluteFilePath absoluteFilePath, CancellationToken cancellationToken = default)
    {
        Directory.CreateDirectory(absoluteFilePath.GetAbsoluteFilePathString());

        return Task.CompletedTask;
    }

    public Task DeleteFileAsync(IAbsoluteFilePath absoluteFilePath, CancellationToken cancellationToken = default)
    {
        File.Delete(
            absoluteFilePath.GetAbsoluteFilePathString());
        
        return Task.CompletedTask;
    }
    
    public Task DeleteDirectoryAsync(IAbsoluteFilePath absoluteFilePath, bool recursive, CancellationToken cancellationToken = default)
    {
        Directory.Delete(
            absoluteFilePath.GetAbsoluteFilePathString(),
            recursive);

        return Task.CompletedTask;
    }

    public bool FileExists(string absoluteFilePathString)
    {
        return File.Exists(absoluteFilePathString);
    }
    
    public void FileCopy(string sourceAbsoluteFilePathString, string destinationAbsoluteFilePathString)
    {
        throw new NotImplementedException();
    }

    public void FileMove(string sourceAbsoluteFilePathString, string destinationAbsoluteFilePathString)
    {
        File.Move(
            sourceAbsoluteFilePathString, 
            destinationAbsoluteFilePathString);
    }

    public DateTime FileGetLastWriteTime(string absoluteFilePathString)
    {
        return File.GetLastWriteTime(absoluteFilePathString);
    }

    public async Task<string> FileReadAllTextAsync(string absoluteFilePathString)
    {
        return await File.ReadAllTextAsync(
            absoluteFilePathString);
    }

    public async Task WriteAllTextAsync(string absoluteFilePathString, string? contents, CancellationToken cancellationToken = default(CancellationToken))
    {
        await File.WriteAllTextAsync(
            absoluteFilePathString,
            contents,
            cancellationToken);
    }

    public bool DirectoryExists(string absoluteFilePathString)
    {
        return Directory.Exists(absoluteFilePathString);
    }

    public void DirectoryMove(string sourceAbsoluteFilePathString, string destinationAbsoluteFilePathString)
    {
        Directory.Move(
            sourceAbsoluteFilePathString, 
            destinationAbsoluteFilePathString);
    }

    public string[] DirectoryGetDirectories(string absoluteFilePathString)
    {
        return Directory.GetDirectories(
            absoluteFilePathString);
    }

    public string[] DirectoryGetFiles(string absoluteFilePathString)
    {
        return Directory
            .GetFiles(absoluteFilePathString);
    }

    public IEnumerable<string> DirectoryEnumerateFileSystemEntries(string absoluteFilePathString)
    {
        return Directory.EnumerateFileSystemEntries(
                absoluteFilePathString);
    }
}