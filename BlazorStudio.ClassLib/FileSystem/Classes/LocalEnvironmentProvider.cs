using BlazorStudio.ClassLib.FileSystem.Interfaces;

namespace BlazorStudio.ClassLib.FileSystem.Classes;

public class LocalEnvironmentProvider : IEnvironmentProvider
{
    public IAbsoluteFilePath RootDirectoryAbsoluteFilePath => new AbsoluteFilePath(
        "/",
        true);

    public IAbsoluteFilePath HomeDirectoryAbsoluteFilePath => new AbsoluteFilePath(
        Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
        true);
}