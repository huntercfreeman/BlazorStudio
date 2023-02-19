namespace BlazorStudio.ClassLib.FileSystem.Interfaces;

public interface IEnvironmentProvider
{
    public IAbsoluteFilePath HomeDirectoryAbsoluteFilePath { get; }
    public IAbsoluteFilePath RootDirectoryAbsoluteFilePath { get; }
}