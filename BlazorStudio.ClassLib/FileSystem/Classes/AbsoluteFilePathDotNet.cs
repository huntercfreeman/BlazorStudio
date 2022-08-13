using System.Text;
using BlazorStudio.ClassLib.FileSystem.Interfaces;

namespace BlazorStudio.ClassLib.FileSystem.Classes;

public class AbsoluteFilePathDotNet : AbsoluteFilePath
{
    public AbsoluteFilePathDotNet(string absoluteFilePathString, bool isDirectory)
        : base(absoluteFilePathString, isDirectory)
    {
    }

    public AbsoluteFilePathDotNet(IAbsoluteFilePath absoluteFilePath, IRelativeFilePath relativeFilePath)
        : base(absoluteFilePath, relativeFilePath)
    {
    }

    public AbsoluteFilePathDotNet(IFileSystemDrive rootDrive, 
        List<IFilePath> directories,
        string fileNameNoExtension, 
        string extensionNoPeriod, 
        bool isDirectory)
            : base(rootDrive, 
                directories,
                fileNameNoExtension, 
                extensionNoPeriod, 
                isDirectory)
    {
    }

    public string Namespace { get;  protected set; }
}