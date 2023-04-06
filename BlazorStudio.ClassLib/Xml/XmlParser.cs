using BlazorStudio.ClassLib.FileSystem.Interfaces;
using BlazorStudio.ClassLib.Namespaces;
using BlazorTextEditor.RazorLib.Analysis;

namespace BlazorStudio.ClassLib.Xml;

/// <summary>TODO: I feel quite silly writing this class because Blazor.Text.Editor has an HTML Lexer which is what I'm effectively looking for. Perhaps this functionality is a duplicate of whats found in HTML Lexer.</summary>
public class XmlParser
{
    public static byte Parse(
        string content,
        NamespacePath namespacePath,
        IEnvironmentProvider environmentProvider)
    {
        var projects = new List<byte>();
        
        var stringWalker = new StringWalker(content);
        
        while (!stringWalker.IsEof)
        {
            
            _ = stringWalker.ReadCharacter();
        }

        return 0;
    }
    
    private static byte ParseGuid(StringWalker stringWalker)
    {
        while (!stringWalker.IsEof)
        {
            
            
            _ = stringWalker.ReadCharacter();
        }

        return 0;
    }
}