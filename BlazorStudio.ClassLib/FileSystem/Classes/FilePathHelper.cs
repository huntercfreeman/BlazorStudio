using BlazorStudio.ClassLib.FileSystem.Interfaces;

namespace BlazorStudio.ClassLib.FileSystem.Classes;

public static class FilePathHelper
{
    public static string StripEndingDirectorySeparatorIfExists(
        string filePath,
        IEnvironmentProvider environmentProvider)
    {
        if (filePath.EndsWith(environmentProvider.DirectorySeparatorChar) ||
            filePath.EndsWith(environmentProvider.AltDirectorySeparatorChar))
        {
            return filePath.Substring(
                    0,
                    filePath.Length - 1);
        }

        return filePath;
    }
}