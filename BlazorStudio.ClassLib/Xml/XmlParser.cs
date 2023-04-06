using BlazorStudio.ClassLib.FileSystem.Interfaces;
using BlazorStudio.ClassLib.Namespaces;
using BlazorTextEditor.RazorLib.Analysis;

namespace BlazorStudio.ClassLib.Xml;

/// <summary>TODO: I feel quite silly writing this class because Blazor.Text.Editor has an HTML Lexer which is what I'm effectively looking for. Perhaps this functionality is a duplicate of whats found in HTML Lexer.</summary>
public class XmlParser
{
    /*
     * class XmlParser
     * abstract class XmlParseStateMachineBase
     * class FileXmlParseStateMachine extends XmlParseStateMachineBase
     * class TagXmlParseStateMachine extends XmlParseStateMachineBase
     * class TagNameXmlParseStateMachine extends XmlParseStateMachineBase
     * class TagAttributeXmlParseStateMachine extends XmlParseStateMachineBase
     * class TagAttibuteNameXmlParseStateMachine extends XmlParseStateMachineBase
     * class TagAttibuteValueXmlParseStateMachine extends XmlParseStateMachineBase
     * class TextXmlParseStateMachine extends XmlParseStateMachineBase
     * class XmlFileModel
     * class XmlTagModel
     * class XmlTextModel extends XmlTagModel
     * class XmlAttributeModel
     */
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
}