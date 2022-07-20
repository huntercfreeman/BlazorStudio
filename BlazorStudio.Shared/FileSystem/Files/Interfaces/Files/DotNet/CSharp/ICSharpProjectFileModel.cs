namespace BlazorStudio.Shared.FileSystem.Files.Interfaces.Files.DotNet.CSharp;

public interface ICSharpProjectFileModel : IFileModel
{
    public Guid GuidOne { get; set; }
    public Guid GuidTwo { get; set; }
    public string CSharpProjectDisplayName { get; set; }
}