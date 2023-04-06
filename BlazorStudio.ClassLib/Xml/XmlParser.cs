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
        /*
         * <div class="mns_blue-background">
         *     Text Node
         *     <span>Text Node</span>
         * </div>
         */
        
        var openTagSyntax = ParseOpenTagSyntax(
            stringWalker,
            xmlDiagnosticBag);
        
        var attributeSyntaxes = ParseAttributeSyntaxesHelper(
            stringWalker,
            xmlDiagnosticBag);

        var contentLength = stringWalker.Content.Length;

        var debuggingAttributes = attributeSyntaxes
            .Select(x =>
            (
                stringWalker.Content.Substring(
                    x.AttributeNameSyntax.TextEditorTextSpan.StartingIndexInclusive,
                    x.AttributeNameSyntax.TextEditorTextSpan.EndingIndexExclusive -
                    x.AttributeNameSyntax.TextEditorTextSpan.StartingIndexInclusive),
                stringWalker.Content.Substring(
                    x.AttributeValueSyntax.TextEditorTextSpan.StartingIndexInclusive,
                    x.AttributeValueSyntax.TextEditorTextSpan.EndingIndexExclusive -
                    x.AttributeValueSyntax.TextEditorTextSpan.StartingIndexInclusive)))
            .ToArray();
        
        var childXmlSyntaxes = ParseChildXmlSyntaxes(
            stringWalker,
            xmlDiagnosticBag);
        
        var closeTagSyntax = ParseCloseTagSyntax(
            stringWalker,
            xmlDiagnosticBag);

        return new TagSyntax(
            openTagSyntax,
            closeTagSyntax,
            attributeSyntaxes,
            childXmlSyntaxes,
            TagKind.Opening);
    }

    private static TagNameSyntax ParseOpenTagSyntax(
        StringWalker stringWalker,
        XmlDiagnosticBag xmlDiagnosticBag)
    {
        var startingPositionIndex = stringWalker.PositionIndex;
        
        var tagNameBuilder = new StringBuilder();
        
        while (!stringWalker.IsEof)
        {
            if (WhitespaceFacts.ALL.Contains(stringWalker.CurrentCharacter) ||
                stringWalker.CheckForSubstring(XmlFacts.OPEN_TAG_WITH_CHILD_CONTENT_ENDING) ||
                stringWalker.CheckForSubstring(XmlFacts.OPEN_TAG_SELF_CLOSING_ENDING))
            {
                break;
            }

            tagNameBuilder.Append(stringWalker.CurrentCharacter);
            
            _ = stringWalker.ReadCharacter();
        }

        var textEditorTextSpan = new TextEditorTextSpan(
            startingPositionIndex,
            stringWalker.PositionIndex,
            0);
        
        return new TagNameSyntax(
            tagNameBuilder.ToString(),
            textEditorTextSpan);
    }
    
    private static ImmutableArray<AttributeSyntax> ParseAttributeSyntaxesHelper(
        StringWalker stringWalker,
        XmlDiagnosticBag xmlDiagnosticBag)
    {
        var startingPositionIndex = stringWalker.PositionIndex;
        var attributeSyntaxes = new List<AttributeSyntax>();
        
        while (!stringWalker.IsEof)
        {
            if (WhitespaceFacts.ALL.Contains(stringWalker.CurrentCharacter))
            {
                _ = stringWalker.ReadCharacter();
                continue;
            }
            
            if (stringWalker.CheckForSubstring(XmlFacts.OPEN_TAG_WITH_CHILD_CONTENT_ENDING) ||
                     stringWalker.CheckForSubstring(XmlFacts.OPEN_TAG_SELF_CLOSING_ENDING))
            {
                break;
            }

            var attributeSyntax = ParseAttributeSyntax(
                stringWalker,
                xmlDiagnosticBag);

            attributeSyntaxes.Add(attributeSyntax);
        }

        return attributeSyntaxes.ToImmutableArray();
    }

    private static AttributeSyntax ParseAttributeSyntax(
        StringWalker stringWalker,
        XmlDiagnosticBag xmlDiagnosticBag)
    {
        var startingPositionIndex = stringWalker.PositionIndex;
        
        int? attributeNameStartingPositionIndexInclusive = null;
        int? attributeNameEndingPositionIndexExclusive = null;
        
        int? attributeValueStartingPositionIndexInclusive = null;
        int? attributeValueEndingPositionIndexExclusive = null;
        
        while (!stringWalker.IsEof)
        {
            if (WhitespaceFacts.ALL.Contains(stringWalker.CurrentCharacter) ||
                stringWalker.CheckForSubstring(XmlFacts.OPEN_TAG_WITH_CHILD_CONTENT_ENDING) ||
                stringWalker.CheckForSubstring(XmlFacts.OPEN_TAG_SELF_CLOSING_ENDING))
            {
                if (attributeNameEndingPositionIndexExclusive is null)
                    attributeNameEndingPositionIndexExclusive = stringWalker.PositionIndex;
                else if (attributeValueEndingPositionIndexExclusive is null)
                {
                    attributeValueEndingPositionIndexExclusive = stringWalker.PositionIndex;
                    break;
                }
            }

            if (stringWalker.CurrentCharacter == XmlFacts.SEPARATOR_FOR_ATTRIBUTE_NAME_AND_ATTRIBUTE_VALUE)
            {
                if (attributeNameEndingPositionIndexExclusive is null)
                    attributeNameEndingPositionIndexExclusive = stringWalker.PositionIndex;
                else
                    break;
            }

            if (attributeNameStartingPositionIndexInclusive is null)
            {
                attributeNameStartingPositionIndexInclusive = stringWalker.PositionIndex;
            }
            else if (attributeValueStartingPositionIndexInclusive is null &&
                     attributeNameEndingPositionIndexExclusive is not null)
            {
                attributeValueStartingPositionIndexInclusive = stringWalker.PositionIndex;
            }
            
            _ = stringWalker.ReadCharacter();
        }

        var attributeNameSyntax = new AttributeNameSyntax(
            new TextEditorTextSpan(
                attributeNameStartingPositionIndexInclusive.Value,
                attributeNameEndingPositionIndexExclusive.Value,
                0));
        
        var attributeValueSyntax = new AttributeValueSyntax(
            new TextEditorTextSpan(
                attributeValueStartingPositionIndexInclusive.Value,
                attributeValueEndingPositionIndexExclusive.Value,
                0));

        var attributeSyntax = new AttributeSyntax(
            attributeNameSyntax,
            attributeValueSyntax);
        
        return attributeSyntax;
    }

    private static ImmutableArray<IXmlSyntax> ParseChildXmlSyntaxes(
        StringWalker stringWalker,
        XmlDiagnosticBag xmlDiagnosticBag)
    {
        /*
         * <div class="mns_blue-background">
         *     Text Node
         *     <span>Text Node</span>
         * </div>
         */

        throw new NotImplementedException();
    }
    
    private static TagNameSyntax ParseCloseTagSyntax(
        StringWalker stringWalker,
        XmlDiagnosticBag xmlDiagnosticBag)
    {
        throw new NotImplementedException();
    }
}