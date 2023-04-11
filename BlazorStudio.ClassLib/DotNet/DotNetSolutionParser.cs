using System.Collections.Immutable;
using System.Text;
using BlazorStudio.ClassLib.DotNet.CSharp;
using BlazorStudio.ClassLib.FileSystem.Classes.FilePath;
using BlazorStudio.ClassLib.FileSystem.Interfaces;
using BlazorStudio.ClassLib.Namespaces;
using BlazorTextEditor.RazorLib.Analysis;

namespace BlazorStudio.ClassLib.DotNet;

public class DotNetSolutionParser
{
    public static DotNetSolution Parse(
        string content,
        NamespacePath namespacePath,
        IEnvironmentProvider environmentProvider)
    {
        var projects = new List<IDotNetProject>();
        var dotNetSolutionFolders = new List<DotNetSolutionFolder>();
        
        var stringWalker = new StringWalker(content);
        
        while (!stringWalker.IsEof)
        {
            if (stringWalker.CheckForSubstring(DotNetSolutionFacts.START_OF_PROJECT_DEFINITION))
            {
                var dotNetProjectSyntax = ParseDotNetProject(stringWalker);

                var projectAbsoluteFilePathString = AbsoluteFilePath
                    .JoinAnAbsoluteFilePathAndRelativeFilePath(
                        namespacePath.AbsoluteFilePath,
                        dotNetProjectSyntax.RelativePathFromSolutionFileString,
                        environmentProvider);

                var projectAbsoluteFilePath = new AbsoluteFilePath(
                    projectAbsoluteFilePathString,
                    false,
                    environmentProvider);
                
                dotNetProjectSyntax.SetAbsoluteFilePath(projectAbsoluteFilePath);
                
                projects.Add(dotNetProjectSyntax);

                if (dotNetProjectSyntax.DotNetProjectKind == DotNetProjectKind.SolutionFolder)
                {
                    dotNetSolutionFolders.Add((DotNetSolutionFolder)dotNetProjectSyntax);
                }
            }
            
            _ = stringWalker.ReadCharacter();
        }

        return new DotNetSolution(
            namespacePath,
            projects.ToImmutableList(),
            dotNetSolutionFolders.ToImmutableList());
    }
    
    private static IDotNetProject ParseDotNetProject(StringWalker stringWalker)
    {
        var projectBuilder = new StringBuilder();
        
        Guid? projectTypeGuid = null;
        Guid? projectIdGuid = null;

        string? displayName = null;
        string? relativePathFromSolutionFileString = null;

        bool withinDoubleQuote = false;
        bool readingProjectDefinitionMember = false;

        var doubleQuoteCounter = 0;

        while (!stringWalker.IsEof)
        {
            if (stringWalker.CheckForSubstring(DotNetSolutionFacts.END_OF_PROJECT_DEFINITION))
            {
                break;
            }
            else if (stringWalker.CheckForSubstring(DotNetSolutionFacts.START_OF_PROJECT_DEFINITION_MEMBER))
            {
                doubleQuoteCounter++;
            }
            else if (WhitespaceFacts.ALL.Contains(stringWalker.CurrentCharacter))
            {
                _ = stringWalker.ReadCharacter();
                continue;
            }
            else if (doubleQuoteCounter > 0)
            {
                if (stringWalker.CheckForSubstring(DotNetSolutionFacts.START_OF_GUID))
                {
                    _ = stringWalker.ReadCharacter();
                    
                    while (!stringWalker.IsEof)
                    {
                        if (WhitespaceFacts.ALL.Contains(stringWalker.CurrentCharacter))
                            _ = stringWalker.ReadCharacter();
                        else
                            break;
                    }
                    
                    var guid = ParseGuid(stringWalker);

                    if (projectTypeGuid is null)
                        projectTypeGuid = guid;
                    else
                        projectIdGuid = guid;
                
                    while (!stringWalker.IsEof)
                    {
                        if (doubleQuoteCounter == 0)
                            break;
                    
                        if (stringWalker.CheckForSubstring(DotNetSolutionFacts.START_OF_PROJECT_DEFINITION_MEMBER))
                            doubleQuoteCounter--;
                    
                        _ = stringWalker.ReadCharacter();
                    }
                }
                else
                {
                    var stringValue = ParseStringValue(stringWalker, doubleQuoteCounter);
                    doubleQuoteCounter = 0;
                        
                    if (displayName is null)
                        displayName = stringValue;
                    else
                        relativePathFromSolutionFileString = stringValue;
                }
            }
            
            _ = stringWalker.ReadCharacter();
        }

        if (projectTypeGuid.Value == DotNetSolutionFolder.SolutionFolderProjectTypeGuid)
        {
            return new DotNetSolutionFolder(
                displayName,
                projectTypeGuid.Value, 
                relativePathFromSolutionFileString,
                projectIdGuid.Value);
        }

        return new CSharpProject(
            displayName,
            projectTypeGuid.Value, 
            relativePathFromSolutionFileString,
            projectIdGuid.Value);
    }
    
    private static Guid ParseGuid(StringWalker stringWalker)
    {
        var guidBuilder = new StringBuilder();
        
        while (!stringWalker.IsEof)
        {
            if (stringWalker.CheckForSubstring(DotNetSolutionFacts.END_OF_PROJECT_DEFINITION))
            {
                break;
            }
            else if (stringWalker.CheckForSubstring(DotNetSolutionFacts.END_OF_GUID))
            {
                break;
            }
            else
            {
                guidBuilder.Append(stringWalker.CurrentCharacter);
            }
            
            _ = stringWalker.ReadCharacter();
        }

        return Guid.Parse(guidBuilder.ToString());
    }
    
    private static string ParseStringValue(StringWalker stringWalker, int doubleQuoteCounter)
    {
        var stringValueBuilder = new StringBuilder();
        
        while (!stringWalker.IsEof)
        {
            if (doubleQuoteCounter == 0)
                break;
            
            if (stringWalker.CheckForSubstring(DotNetSolutionFacts.END_OF_PROJECT_DEFINITION_MEMBER))
            {
                break;
            }
            else if (stringWalker.CheckForSubstring(DotNetSolutionFacts.END_OF_PROJECT_DEFINITION))
            {
                doubleQuoteCounter--;
            }
            else
            {
                stringValueBuilder.Append(stringWalker.CurrentCharacter);
            }
            
            _ = stringWalker.ReadCharacter();
        }

        return stringValueBuilder.ToString();
    }
}