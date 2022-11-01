using BlazorStudio.ClassLib.FileSystem.Interfaces;

namespace BlazorStudio.ClassLib.CommandLine;

public static class DotNetCliFacts
{
    public const string DotnetNewSlnCommand = "dotnet new sln";
    
    public static string FormatStartProjectWithoutDebugging(IAbsoluteFilePath projectAbsoluteFilePath)
    {
        return FormatStartProjectWithoutDebugging(
            projectAbsoluteFilePath.GetAbsoluteFilePathString());
    }
    
    public static string FormatStartProjectWithoutDebugging(string projectPath)
    {
        return $"dotnet run --project {projectPath}";
    }
    
    
}