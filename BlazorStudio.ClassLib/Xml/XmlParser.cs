using System.Collections.Immutable;
using System.Text;
using System.Xml;
using BlazorStudio.ClassLib.FileSystem.Interfaces;
using BlazorStudio.ClassLib.Namespaces;
using BlazorStudio.ClassLib.Xml.Facts;
using BlazorStudio.ClassLib.Xml.SyntaxEnums;
using BlazorStudio.ClassLib.Xml.SyntaxObjects;
using BlazorTextEditor.RazorLib.Analysis;
using BlazorTextEditor.RazorLib.Lexing;

namespace BlazorStudio.ClassLib.Xml;

/// <summary>TODO: I feel quite silly writing this class because Blazor.Text.Editor has an HTML Lexer which is what I'm effectively looking for. Perhaps this functionality is a duplicate of whats found in HTML Lexer.</summary>
public class XmlParser
{
    public static XmlSyntaxUnit Parse(
        string content,
        NamespacePath namespacePath,
        IEnvironmentProvider environmentProvider)
    {
        var xmlDiagnosticBag = new XmlDiagnosticBag();
        
        var topLevelChildren = new List<TagSyntax>();
        
        var stringWalker = new StringWalker(content);
        
        while (!stringWalker.IsEof)
        {
            if (stringWalker.CurrentCharacter == XmlFacts.OPEN_TAG_BEGINNING)
            {
                var tag = ParseTag(
                    stringWalker,
                    xmlDiagnosticBag);

                topLevelChildren.Add(tag);
            }
            
            _ = stringWalker.ReadCharacter();
        }

        var rootOpenTagNameSyntax = new TagNameSyntax(
            "document",
            new TextEditorTextSpan(
                0,
                0,
                0));
        
        var rootCloseTagNameSyntax = new TagNameSyntax(
            "document",
            new TextEditorTextSpan(
                0,
                0,
                0));
        
        var rootTagSyntax = new TagSyntax(
            rootOpenTagNameSyntax,
            rootCloseTagNameSyntax,
            ImmutableArray<AttributeSyntax>.Empty,
            topLevelChildren
                .Select(x => (IXmlSyntax)x)
                .ToImmutableArray(),
            TagKind.Opening);
        
        return new XmlSyntaxUnit(
            rootTagSyntax,
            xmlDiagnosticBag);
    }
    
    private static TagSyntax ParseTag(
        StringWalker stringWalker,
        XmlDiagnosticBag xmlDiagnosticBag)
    {
        var tagNameBuilder = new StringBuilder();
        
        while (!stringWalker.IsEof)
        {
            
            
            _ = stringWalker.ReadCharacter();
        }

        var openTagSyntax = ParseOpenTagSyntax();
        
        var attributeSyntaxes = ParseAttributeSyntaxes();
        
        var childXmlSyntaxes = ParseChildXmlSyntaxes();
        
        var closeTagSyntax = ParseCloseTagSyntax();

        return new TagSyntax(
            openTagSyntax,
            closeTagSyntax,
            attributeSyntaxes,
            childXmlSyntaxes,
            TagKind.Opening);
    }

    private static TagNameSyntax ParseOpenTagSyntax()
    {
        throw new NotImplementedException();
    }
    
    private static ImmutableArray<AttributeSyntax> ParseAttributeSyntaxes()
    {
        throw new NotImplementedException();
    }
    
    private static ImmutableArray<IXmlSyntax> ParseChildXmlSyntaxes()
    {
        throw new NotImplementedException();
    }
    
    private static TagNameSyntax ParseCloseTagSyntax()
    {
        throw new NotImplementedException();
    }
}