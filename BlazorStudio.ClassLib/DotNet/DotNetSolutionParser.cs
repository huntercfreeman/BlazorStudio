using System.Collections.Immutable;
using System.Text;
using BlazorStudio.ClassLib.DotNet.CSharp;
using BlazorTextEditor.RazorLib.Analysis;

namespace BlazorStudio.ClassLib.DotNet;

public class DotNetSolutionParser
{
    public static DotNetSolution Parse(string content)
    {
        var projects = new List<IDotNetProject>();
        
        var stringWalker = new StringWalker(content);
        
        while (!stringWalker.IsEof)
        {
            if (stringWalker.CheckForSubstring(DotNetSolutionFacts.START_OF_PROJECT_DEFINITION))
            {
                var dotNetProjectSyntax = ParseDotNetProject(stringWalker);
                
                projects.Add(dotNetProjectSyntax);
            }
            
            _ = stringWalker.ReadCharacter();
        }

        return new DotNetSolution("TODO: DisplayName", projects.ToImmutableList());
    }
    
    private static IDotNetProject ParseDotNetProject(StringWalker stringWalker)
    {
        var projectBuilder = new StringBuilder();
        
        while (!stringWalker.IsEof)
        {
            if (WhitespaceFacts.LINE_ENDING_CHARACTERS.Contains(stringWalker.CurrentCharacter))
            {
                break;
            }
            else
            {
                projectBuilder.Append(stringWalker.CurrentCharacter);
            }
            
            _ = stringWalker.ReadCharacter();
        }

        return new CSharpProject(projectBuilder.ToString(), Guid.Empty,  Guid.Empty);
    }
}