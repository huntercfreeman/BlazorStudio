using BlazorStudio.ClassLib.FileSystem.Interfaces;

namespace BlazorStudio.ClassLib.FileSystem.Classes;

public class WebsiteEnvironmentProvider : IEnvironmentProvider
{
    public IAbsoluteFilePath RootDirectoryAbsoluteFilePath => new AbsoluteFilePath(
        "/",
        true);

    public IAbsoluteFilePath HomeDirectoryAbsoluteFilePath => new AbsoluteFilePath(
        Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
        true);
}