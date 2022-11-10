namespace BlazorStudio.ClassLib.FileSystem.Interfaces;

/// <summary>
/// TODO: I was not aware of <see cref="System.IO.DirectoryInfo"/> nor <see cref="System.IO.FileInfo"/> when I started this project. I will when I find time remedy this mistake and change over to Microsoft's file system classes.
/// </summary>
public interface IAbsoluteFilePath : IFilePath
{
    /// <summary>
    /// Only non null when specifying a drive to mount
    ///
    /// Example: "C:\\" gets stored as "C"
    ///
    /// To get the guaranteed non null root directory
    /// use GetRootDirectory
    /// </summary>
    public IFileSystemDrive? RootDrive { get; }
    /// <summary>
    /// Returns either System.IO.Path.DirectorySeparatorChar
    ///
    /// OR
    ///
    /// Returns $"{RootDrive}:{System.IO.Path.DirectorySeparatorChar}"
    /// </summary>
    public string GetRootDirectory { get; }
    public string GetAbsoluteFilePathString();
    public AbsoluteFilePathKind AbsoluteFilePathKind { get; }
}