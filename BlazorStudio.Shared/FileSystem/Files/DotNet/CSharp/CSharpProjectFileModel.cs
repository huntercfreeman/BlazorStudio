using BlazorStudio.Shared.FileSystem.Files.Interfaces.Files.DotNet.CSharp;
using BlazorStudio.Shared.FileSystem.Interfaces;

namespace BlazorStudio.Shared.FileSystem.Files.DotNet.CSharp;

public class CSharpProjectFileModel : FileModelBase, ICSharpProjectFileModel
{
    public CSharpProjectFileModel(IFileDescriptor? fileDescriptor,
        IAbsoluteFilePath absoluteFilePath,
        Guid guidOne,
        Guid guidTwo,
        string cSharpProjectDisplayName)
        : base(fileDescriptor, absoluteFilePath)
    {
        GuidOne = guidOne;
        GuidTwo = guidTwo;
        CSharpProjectDisplayName = cSharpProjectDisplayName;
    }

    public Guid GuidOne { get; set; }
    public Guid GuidTwo { get; set; }
    public string CSharpProjectDisplayName { get; set; }
}