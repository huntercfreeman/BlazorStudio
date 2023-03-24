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
            }
            
            _ = stringWalker.ReadCharacter();
        }

        return new DotNetSolution(namespacePath, projects.ToImmutableList());
    }
    
    /*
     * Microsoft Visual Studio Solution File, Format Version 12.00
Project(""{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}"") = ""BlazorApp1"", ""BlazorApp1\BlazorApp1.csproj"", ""{510BA6A0-B1B5-4D57-AB74-EA7ED2127ED4}""
EndProject
Project(""{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}"") = ""BlazorCommon.RazorLib"", ""..\BlazorCommon\BlazorCommon.RazorLib\BlazorCommon.RazorLib.csproj"", ""{2F763B00-22EC-4566-B27A-C3D6367AC8F0}""
EndProject
Global

    -------------------
    
    Every value for a Project definition within a .sln is wrapped in quotes.
    
    First algorithm
        -Assume the order is always the same
            -ProjectTypeGuid // Find '"' -> '{'
            -DisplayName // Find '"' -> ~'{'
            -RelativePathFromSolutionFile // Find '"' -> ~'{'
            -ProjectIdGuid // Find '"' -> '{'
     */
    
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