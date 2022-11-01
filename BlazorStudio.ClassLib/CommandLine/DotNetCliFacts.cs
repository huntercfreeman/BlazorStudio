using BlazorStudio.ClassLib.FileSystem.Interfaces;

namespace BlazorStudio.ClassLib.CommandLine;

public static class DotNetCliFacts
{
    public static string StartProjectWithoutDebugging(IAbsoluteFilePath projectAbsoluteFilePath)
    {
        return StartProjectWithoutDebugging(
            projectAbsoluteFilePath.GetAbsoluteFilePathString());
    }
    
    public static string StartProjectWithoutDebugging(string projectPath)
    {
        return $"dotnet run --project {projectPath}";
    }
}