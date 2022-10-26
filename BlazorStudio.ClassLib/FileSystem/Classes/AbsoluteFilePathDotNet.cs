using BlazorStudio.ClassLib.FileSystem.Interfaces;
using Microsoft.CodeAnalysis;

namespace BlazorStudio.ClassLib.FileSystem.Classes;

public class AbsoluteFilePathDotNet : AbsoluteFilePath
{
    public AbsoluteFilePathDotNet(string absoluteFilePathString, bool isDirectory, ProjectId? projectId)
        : base(absoluteFilePathString, isDirectory)
    {
        ProjectId = projectId;
    }

    // ReSharper disable once UnusedMember.Global
    public AbsoluteFilePathDotNet(IAbsoluteFilePath absoluteFilePath, IRelativeFilePath relativeFilePath,
        ProjectId projectId)
        : base(absoluteFilePath, relativeFilePath)
    {
        ProjectId = projectId;
    }

    // ReSharper disable once UnusedMember.Global
    public AbsoluteFilePathDotNet(IFileSystemDrive rootDrive,
        List<IFilePath> directories,
        string fileNameNoExtension,
        string extensionNoPeriod,
        bool isDirectory,
        ProjectId projectId)
        : base(rootDrive,
            directories,
            fileNameNoExtension,
            extensionNoPeriod,
            isDirectory)
    {
        ProjectId = projectId;
    }

    /// <summary>
    /// The project the file belongs to. (this AbsoluteFilePathDotNet can also be the project itself)
    /// </summary>
    public ProjectId? ProjectId { get; }
    public override AbsoluteFilePathKind AbsoluteFilePathKind => AbsoluteFilePathKind.DotNet;
}