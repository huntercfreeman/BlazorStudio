using BlazorStudio.ClassLib.FileSystem.Interfaces;

namespace BlazorStudio.ClassLib.CommandLine;

/// <summary>
/// Any parameters given will be wrapped in quotes internally
/// </summary>
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
        projectPath = QuoteParameter(projectPath);
        
        return $"dotnet run --project {projectPath}";
    }
    
    private static string QuoteParameter(string parameter)
    {
        return $"\"{parameter}\"";
    }
}