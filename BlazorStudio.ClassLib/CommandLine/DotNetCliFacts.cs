using BlazorStudio.ClassLib.FileSystem.Interfaces;

namespace BlazorStudio.ClassLib.CommandLine;

/// <summary>
/// Any values given will be wrapped in quotes internally
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
        projectPath = CommandLineHelper.QuoteValue(projectPath);
        
        return $"dotnet run --project {projectPath}";
    }
    
    public static string FormatDotnetNewSln(string solutionName)
    {
        solutionName = CommandLineHelper.QuoteValue(solutionName);
        
        return $"{DotnetNewSlnCommand} -o {solutionName}";
    }
    
    public static string FormatDotnetNewCSharpProject(
        string projectTemplateName, 
        string cSharpProjectName, 
        string optionalParameters)
    {
        projectTemplateName = CommandLineHelper.QuoteValue(projectTemplateName);
        cSharpProjectName = CommandLineHelper.QuoteValue(cSharpProjectName);
        
        return $"dotnet new {projectTemplateName} -o {cSharpProjectName} {optionalParameters}";
    }
    
    public static string FormatAddExistingProjectToSolution(
        string solutionAbsoluteFilePathString, 
        string cSharpProjectPath)
    {
        solutionAbsoluteFilePathString = CommandLineHelper.QuoteValue(solutionAbsoluteFilePathString);
        cSharpProjectPath = CommandLineHelper.QuoteValue(cSharpProjectPath);
        
        return $"dotnet sln {solutionAbsoluteFilePathString} add {cSharpProjectPath}";
    }
    
    public static string FormatRemoveCSharpProjectReferenceFromSolutionAction(
        string solutionAbsoluteFilePathString, 
        string cSharpProjectAbsoluteFilePathString)
    {
        solutionAbsoluteFilePathString = CommandLineHelper.QuoteValue(solutionAbsoluteFilePathString);
        cSharpProjectAbsoluteFilePathString = CommandLineHelper.QuoteValue(cSharpProjectAbsoluteFilePathString);
        
        return $"dotnet sln {solutionAbsoluteFilePathString} remove {cSharpProjectAbsoluteFilePathString}";
    }
    
    public static string FormatAddNugetPackageReferenceToProject(
        string cSharpProjectAbsoluteFilePathString, 
        string nugetPackageId,
        string nugetPackageVersion)
    {
        cSharpProjectAbsoluteFilePathString = CommandLineHelper.QuoteValue(cSharpProjectAbsoluteFilePathString);
        nugetPackageId = CommandLineHelper.QuoteValue(nugetPackageId);
        nugetPackageVersion = CommandLineHelper.QuoteValue(nugetPackageVersion);
        
        return $"dotnet add {cSharpProjectAbsoluteFilePathString} package {nugetPackageId} --version {nugetPackageVersion}";
    }
    
    public static string FormatAddProjectToProjectReference(
        string receivingProjectAbsoluteFilePathString, 
        string referencedProjectAbsoluteFilePathString)
    {
        receivingProjectAbsoluteFilePathString = CommandLineHelper.QuoteValue(receivingProjectAbsoluteFilePathString);
        referencedProjectAbsoluteFilePathString = CommandLineHelper.QuoteValue(referencedProjectAbsoluteFilePathString);
        
        return $"dotnet add {receivingProjectAbsoluteFilePathString} reference {referencedProjectAbsoluteFilePathString}";
    }
    
    public static string FormatRemoveProjectToProjectReference(
        string modifyProjectAbsoluteFilePathString, 
        string referenceProjectAbsoluteFilePathString)
    {
        modifyProjectAbsoluteFilePathString = CommandLineHelper.QuoteValue(modifyProjectAbsoluteFilePathString);
        referenceProjectAbsoluteFilePathString = CommandLineHelper.QuoteValue(referenceProjectAbsoluteFilePathString);
        
        return $"dotnet remove {modifyProjectAbsoluteFilePathString} reference {referenceProjectAbsoluteFilePathString}";
    }
}