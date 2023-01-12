namespace BlazorStudio.ClassLib.FileSystem.Classes;

public static class FilePathHelper
{
    public static string StripEndingDirectorySeparatorIfExists(string filePath)
    {
        if (filePath.EndsWith(Path.DirectorySeparatorChar) ||
            filePath.EndsWith(Path.AltDirectorySeparatorChar))
        {
            return filePath.Substring(
                    0,
                    filePath.Length - 1);
        }

        return filePath;
    }
}