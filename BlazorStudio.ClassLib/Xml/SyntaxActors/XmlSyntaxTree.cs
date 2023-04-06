using System.Collections.Immutable;
using System.Text;
using BlazorTextEditor.RazorLib.Analysis;
using BlazorTextEditor.RazorLib.Analysis.Html;
using BlazorTextEditor.RazorLib.Analysis.Html.Decoration;
using BlazorTextEditor.RazorLib.Analysis.Html.ExtensionMethods;
using BlazorTextEditor.RazorLib.Analysis.Html.Facts;
using BlazorTextEditor.RazorLib.Analysis.Html.InjectedLanguage;
using BlazorTextEditor.RazorLib.Analysis.Html.SyntaxEnums;
using BlazorTextEditor.RazorLib.Analysis.Html.SyntaxObjects;
using BlazorTextEditor.RazorLib.Lexing;

namespace BlazorStudio.ClassLib.Xml.SyntaxActors;

public static class XmlSyntaxTree
{
    public static XmlSyntaxUnit ParseText(
        string content,
        InjectedLanguageDefinition? injectedLanguageDefinition = null)
    {
        var stringWalker = new StringWalker(content);

        var rootTagSyntaxBuilder = new TagSyntax.TagSyntaxBuilder
        {
            OpenTagNameSyntax = new TagNameSyntax(
                "document",
                new TextEditorTextSpan(
                    0,
                    0,
                    (byte)HtmlDecorationKind.None)),
        };

        var textEditorHtmlDiagnosticBag = new TextEditorHtmlDiagnosticBag();

        rootTagSyntaxBuilder.ChildHtmlSyntaxes = HtmlSyntaxTreeStateMachine
            .ParseTagChildContent(
                stringWalker,
                textEditorHtmlDiagnosticBag,
                injectedLanguageDefinition);

        var htmlSyntaxUnitBuilder = new XmlSyntaxUnit.HtmlSyntaxUnitBuilder(
            rootTagSyntaxBuilder.Build(),
            textEditorHtmlDiagnosticBag);

        return htmlSyntaxUnitBuilder.Build();
    }

    public static class HtmlSyntaxTreeStateMachine
    {
        /// <summary>
        ///     Invocation of this method requires the
        ///     stringWalker to have <see cref="StringWalker.PeekCharacter" />
        ///     of 0 be equal to
        ///     <see cref="HtmlFacts.OPEN_TAG_BEGINNING" />
        /// </summary>
        public static TagSyntax ParseTag(
            StringWalker stringWalker,
            TextEditorHtmlDiagnosticBag textEditorHtmlDiagnosticBag,
            InjectedLanguageDefinition? injectedLanguageDefinition)
        {
            if (stringWalker.CheckForSubstring(
                    HtmlFacts.COMMENT_TAG_BEGINNING))
            {
                return ParseComment(
                    stringWalker,
                    textEditorHtmlDiagnosticBag,
                    injectedLanguageDefinition);
            }
            
            var startingPositionIndex = stringWalker.PositionIndex;

            var tagBuilder = new TagSyntax.TagSyntaxBuilder();

            // HtmlFacts.TAG_OPENING_CHARACTER
            _ = stringWalker.ReadCharacter();

            
            
            // Example: <!DOCTYPE html>
            if (stringWalker.PeekCharacter(0) == HtmlFacts.SPECIAL_HTML_TAG)
            {
                // HtmlFacts.SPECIAL_HTML_TAG_CHARACTER
                stringWalker.ReadCharacter();

                tagBuilder.HasSpecialHtmlCharacter = true;
            }

            tagBuilder.OpenTagNameSyntax = ParseTagName(
                stringWalker,
                textEditorHtmlDiagnosticBag,
                injectedLanguageDefinition);

            // Get all html attributes
            // break when see End Of File or
            // closing of the tag
            while (true)
            {
                // Skip Whitespace
                while (!stringWalker.IsEof)
                {
                    if (WhitespaceFacts.ALL.Contains(stringWalker.CurrentCharacter))
                        _ = stringWalker.ReadCharacter();
                    else
                        break;
                }

                // End Of File is unexpected at this point so report a diagnostic.
                if (stringWalker.CurrentCharacter == ParserFacts.END_OF_FILE)
                {
                    textEditorHtmlDiagnosticBag.ReportEndOfFileUnexpected(
                        new TextEditorTextSpan(
                            startingPositionIndex,
                            stringWalker.PositionIndex,
                            (byte)HtmlDecorationKind.Error));

                    return tagBuilder.Build();
                }

                if (stringWalker.CheckForSubstring(HtmlFacts.OPEN_TAG_WITH_CHILD_CONTENT_ENDING))
                {
                    // Ending of opening tag
                    tagBuilder.TagKind = TagKind.Opening;

                    // Skip the '>' character to set stringWalker at the first
                    // character of the child content
                    _ = stringWalker.ReadCharacter();

                    tagBuilder.ChildHtmlSyntaxes = ParseTagChildContent(
                        stringWalker,
                        textEditorHtmlDiagnosticBag,
                        injectedLanguageDefinition);

                    // At the closing tag now so check that the closing tag
                    // name matches the opening tag.
                    //
                    // An opening tag of
                    //     <div>
                    // Should have a matching closing tag of
                    //     </div>

                    // TODO: check that the closing tag name matches the opening tag
                }
                else if (stringWalker.CheckForSubstring(HtmlFacts.OPEN_TAG_SELF_CLOSING_ENDING))
                {
                    _ = stringWalker.ReadRange(
                        HtmlFacts.OPEN_TAG_SELF_CLOSING_ENDING
                            .Length);

                    // Ending of self-closing tag
                    tagBuilder.TagKind = TagKind.SelfClosing;

                    return tagBuilder.Build();
                }
                else if (stringWalker.CheckForSubstring(HtmlFacts.CLOSE_TAG_WITH_CHILD_CONTENT_BEGINNING))
                {
                    _ = stringWalker.ReadRange(
                        HtmlFacts.CLOSE_TAG_WITH_CHILD_CONTENT_BEGINNING
                            .Length);

                    var closeTagNameStartingPositionIndex = stringWalker.PositionIndex;

                    var closeTagNameBuilder = new StringBuilder();

                    stringWalker.WhileNotEndOfFile(() =>
                    {
                        if (stringWalker.CheckForSubstring(
                                HtmlFacts.CLOSE_TAG_WITH_CHILD_CONTENT_ENDING))
                        {
                            tagBuilder.CloseTagNameSyntax = new TagNameSyntax(
                                closeTagNameBuilder.ToString(),
                                new TextEditorTextSpan(
                                    closeTagNameStartingPositionIndex,
                                    stringWalker.PositionIndex,
                                    (byte)HtmlDecorationKind.TagName));

                            _ = stringWalker.ReadRange(
                                HtmlFacts.CLOSE_TAG_WITH_CHILD_CONTENT_ENDING
                                    .Length);

                            return true;
                        }

                        closeTagNameBuilder.Append(stringWalker.CurrentCharacter);
                        return false;
                    });

                    if (tagBuilder.CloseTagNameSyntax is null)
                    {
                        // TODO: Not sure if this can happen but I am getting a warning
                        // about this and aim to get to this when I find time.
                        throw new NotImplementedException();
                    }

                    if (tagBuilder.OpenTagNameSyntax.Value != tagBuilder.CloseTagNameSyntax.Value)
                    {
                        textEditorHtmlDiagnosticBag.ReportOpenTagWithUnMatchedCloseTag(
                            tagBuilder.OpenTagNameSyntax.Value,
                            tagBuilder.CloseTagNameSyntax.Value,
                            new TextEditorTextSpan(
                                closeTagNameStartingPositionIndex,
                                stringWalker.PositionIndex,
                                (byte)HtmlDecorationKind.Error));
                    }

                    return tagBuilder.Build();
                }
                else
                {
                    // Attribute
                    var attributeSyntax = ParseAttribute(
                        stringWalker,
                        textEditorHtmlDiagnosticBag,
                        injectedLanguageDefinition);
                    
                    tagBuilder.AttributeSyntaxes.Add(attributeSyntax);
                }
            }
        }

        /// <summary>
        ///     Invocation of this method requires the
        ///     stringWalker to have <see cref="StringWalker.PeekCharacter" />
        ///     of 0 be equal to the first
        ///     character that is part of the tag's name
        /// </summary>
        public static TagNameSyntax ParseTagName(
            StringWalker stringWalker,
            TextEditorHtmlDiagnosticBag textEditorHtmlDiagnosticBag,
            InjectedLanguageDefinition? injectedLanguageDefinition)
        {
            var startingPositionIndex = stringWalker.PositionIndex;

            var tagNameBuilder = new StringBuilder();

            stringWalker.WhileNotEndOfFile(() =>
            {
                if (stringWalker.CheckForSubstringRange(
                        HtmlFacts.TAG_NAME_STOP_DELIMITERS,
                        out var matchedOn))
                    return true;

                tagNameBuilder.Append(stringWalker.CurrentCharacter);

                return false;
            });

            var tagName = tagNameBuilder.ToString();

            if (tagNameBuilder.Length == 0)
            {
                if (stringWalker.CurrentCharacter == ParserFacts.END_OF_FILE)
                {
                    textEditorHtmlDiagnosticBag.ReportEndOfFileUnexpected(
                        new TextEditorTextSpan(
                            startingPositionIndex,
                            stringWalker.PositionIndex,
                            (byte)HtmlDecorationKind.Error));
                }
                else
                {
                    // Report a diagnostic for the missing 'tag name'
                    textEditorHtmlDiagnosticBag.ReportTagNameMissing(
                        new TextEditorTextSpan(
                            startingPositionIndex,
                            stringWalker.PositionIndex,
                            (byte)HtmlDecorationKind.Error));

                    // Fabricate a value for the string variable: 'tagName' so the
                    // rest of the file can still be parsed.
                    tagName =
                        $"__{nameof(textEditorHtmlDiagnosticBag.ReportTagNameMissing)}__";
                }
            }

            return new TagNameSyntax(
                tagName,
                new TextEditorTextSpan(
                    startingPositionIndex,
                    stringWalker.PositionIndex,
                    (byte)HtmlDecorationKind.TagName));
        }

        /*
         * <div>
         *     TEXT NODE: THIS IS THE CHILD CONTENT
         *     HtmlElement to parse: <HtmlElement/>
         *
         *     @myVariable
         * </div>
         */
            
        public static List<IXmlSyntax> ParseTagChildContent(
            StringWalker stringWalker,
            TextEditorHtmlDiagnosticBag textEditorHtmlDiagnosticBag,
            InjectedLanguageDefinition? injectedLanguageDefinition)
        {
            var startingPositionIndex = stringWalker.PositionIndex;

            List<IXmlSyntax> htmlSyntaxes = new();

            var textNodeBuilder = new StringBuilder();

            // Make a TagTextSyntax - HTML TextNode
            // if there was anything in the current builder
            void AddTextNode()
            {
                if (textNodeBuilder.Length <= 0)
                    return;

                var tagTextSyntax = new TagTextSyntax(
                    ImmutableArray<AttributeSyntax>.Empty,
                    ImmutableArray<IXmlSyntax>.Empty,
                    textNodeBuilder.ToString());

                htmlSyntaxes.Add(tagTextSyntax);
                textNodeBuilder.Clear();
            }

            stringWalker.WhileNotEndOfFile(() =>
            {
                if (stringWalker.CheckForSubstring(
                        HtmlFacts.CLOSE_TAG_WITH_CHILD_CONTENT_BEGINNING))
                    return true;

                if (stringWalker.CurrentCharacter ==
                    HtmlFacts.OPEN_TAG_BEGINNING)
                {
                    // If there is text in textNodeBuilder
                    // add a new TextNode to the List of TagSyntax
                    AddTextNode();

                    if (stringWalker.CheckForSubstring(
                            HtmlFacts.COMMENT_TAG_BEGINNING))
                    {
                        htmlSyntaxes.Add(
                            ParseComment(
                                stringWalker,
                                textEditorHtmlDiagnosticBag,
                                injectedLanguageDefinition));
                    }
                    else
                    {
                        htmlSyntaxes.Add(
                            ParseTag(
                                stringWalker,
                                textEditorHtmlDiagnosticBag,
                                injectedLanguageDefinition));
                    }

                    return false;
                }

                if (injectedLanguageDefinition is not null && stringWalker
                        .CheckForInjectedLanguageCodeBlockTag(injectedLanguageDefinition))
                {
                    // If there is text in textNodeBuilder
                    // add a new TextNode to the List of TagSyntax
                    AddTextNode();

                    htmlSyntaxes.AddRange(
                        ParseInjectedLanguageCodeBlock(
                            stringWalker,
                            textEditorHtmlDiagnosticBag,
                            injectedLanguageDefinition));

                    return false;
                }

                textNodeBuilder.Append(stringWalker.CurrentCharacter);
                return false;
            });

            if (stringWalker.CurrentCharacter == ParserFacts.END_OF_FILE)
            {
                textEditorHtmlDiagnosticBag.ReportEndOfFileUnexpected(
                    new TextEditorTextSpan(
                        startingPositionIndex,
                        stringWalker.PositionIndex,
                        (byte)HtmlDecorationKind.Error));
            }

            // If there is text in textNodeBuilder
            // add a new TextNode to the List of TagSyntax
            AddTextNode();

            return htmlSyntaxes;
        }

        public static List<TagSyntax> ParseInjectedLanguageCodeBlock(
            StringWalker stringWalker,
            TextEditorHtmlDiagnosticBag textEditorHtmlDiagnosticBag,
            InjectedLanguageDefinition injectedLanguageDefinition)
        {
            var injectedLanguageFragmentSyntaxes = new List<TagSyntax>();

            var injectedLanguageFragmentSyntaxStartingPositionIndex = stringWalker.PositionIndex;

            // Track text span of the "@" sign (example in .razor files)
            injectedLanguageFragmentSyntaxes.Add(
                new InjectedLanguageFragmentSyntax(
                    ImmutableArray<IXmlSyntax>.Empty,
                    string.Empty,
                    new TextEditorTextSpan(
                        injectedLanguageFragmentSyntaxStartingPositionIndex,
                        stringWalker.PositionIndex + 1,
                        (byte)HtmlDecorationKind.InjectedLanguageFragment)));

            injectedLanguageFragmentSyntaxes.AddRange(
                injectedLanguageDefinition.ParseInjectedLanguageFunc
                    .Invoke(
                        stringWalker,
                        textEditorHtmlDiagnosticBag,
                        injectedLanguageDefinition));

            return injectedLanguageFragmentSyntaxes;
        }
        
        public static AttributeSyntax ParseAttribute(
            StringWalker stringWalker,
            TextEditorHtmlDiagnosticBag textEditorHtmlDiagnosticBag,
            InjectedLanguageDefinition? injectedLanguageDefinition)
        {
            var attributeNameSyntax = ParseAttributeName(
                stringWalker,
                textEditorHtmlDiagnosticBag,
                injectedLanguageDefinition);
            
            _ = TryReadAttributeValue(
                    stringWalker,
                    textEditorHtmlDiagnosticBag,
                    injectedLanguageDefinition,
                    out var attributeValueSyntax);

            return new AttributeSyntax(
                attributeNameSyntax,
                attributeValueSyntax);
        }

        /// <summary>
        /// currentCharacterIn:<br/>
        /// -Any character that can start an attribute name<br/>
        /// currentCharacterOut:<br/>
        /// -<see cref="WhitespaceFacts.ALL"/> (whitespace)<br/>
        /// -<see cref="HtmlFacts.SEPARATOR_FOR_ATTRIBUTE_NAME_AND_ATTRIBUTE_VALUE"/><br/>
        /// -<see cref="HtmlFacts.OPEN_TAG_ENDING_OPTIONS"/><br/>
        /// </summary>
        public static AttributeNameSyntax ParseAttributeName(
            StringWalker stringWalker,
            TextEditorHtmlDiagnosticBag textEditorHtmlDiagnosticBag,
            InjectedLanguageDefinition? injectedLanguageDefinition)
        {
            // When ParseAttributeName is invoked
            // the PositionIndex
            // is always 1 character too far
            _ = stringWalker.BacktrackCharacter();

            var startingPositionIndex = stringWalker.PositionIndex;
            
            while (!stringWalker.IsEof)
            {
                _ = stringWalker.ReadCharacter();

                if (WhitespaceFacts.ALL.Contains(stringWalker.CurrentCharacter) ||
                    HtmlFacts.SEPARATOR_FOR_ATTRIBUTE_NAME_AND_ATTRIBUTE_VALUE == stringWalker.CurrentCharacter ||
                    stringWalker.CheckForSubstringRange(HtmlFacts.OPEN_TAG_ENDING_OPTIONS, out var matchedOn))
                {
                    break;
                }
            }

            var attributeNameTextSpan = new TextEditorTextSpan(
                startingPositionIndex,
                stringWalker.PositionIndex,
                (byte)HtmlDecorationKind.AttributeName);
            
            return new AttributeNameSyntax(attributeNameTextSpan);
        }
        
        /// <summary>
        /// Returns placeholder match attribute value if
        /// fails to read an attribute value<br/>
        /// <br/>
        /// currentCharacterIn:<br/>
        /// -<see cref="WhitespaceFacts.ALL"/> (whitespace)<br/>
        /// -<see cref="HtmlFacts.SEPARATOR_FOR_ATTRIBUTE_NAME_AND_ATTRIBUTE_VALUE"/><br/>
        /// -<see cref="HtmlFacts.OPEN_TAG_ENDING_OPTIONS"/><br/>
        /// currentCharacterOut:<br/>
        /// -<see cref="HtmlFacts.ATTRIBUTE_VALUE_ENDING"/><br/>
        /// -<see cref="HtmlFacts.OPEN_TAG_ENDING_OPTIONS"/><br/>
        /// </summary>
        private static bool TryReadAttributeValue(
            StringWalker stringWalker,
            TextEditorHtmlDiagnosticBag textEditorHtmlDiagnosticBag,
            InjectedLanguageDefinition? injectedLanguageDefinition,
            out AttributeValueSyntax attributeValueSyntax)
        {
            if (WhitespaceFacts.ALL.Contains(stringWalker.CurrentCharacter))
            {
                // Move to the first non-whitespace
                while (!stringWalker.IsEof)
                {
                    _ = stringWalker.ReadCharacter();

                    if (!WhitespaceFacts.ALL.Contains(stringWalker.CurrentCharacter))
                        break;
                }
            }
            
            if (HtmlFacts.SEPARATOR_FOR_ATTRIBUTE_NAME_AND_ATTRIBUTE_VALUE == stringWalker.CurrentCharacter)
            {
                attributeValueSyntax = ParseAttributeValue(
                    stringWalker,
                    textEditorHtmlDiagnosticBag,
                    injectedLanguageDefinition);
                
                return true;
            }

            // Set out variable as a 'matched attribute value' so there aren't
            // any cascading error diagnostics due to having expected an attribute value.
            var attributeValueTextSpan = new TextEditorTextSpan(
                0,
                0,
                (byte)HtmlDecorationKind.AttributeValue);

            attributeValueSyntax = new AttributeValueSyntax(attributeValueTextSpan);
            
            return false;
        }
        
        /// <summary>
        /// currentCharacterIn:<br/>
        /// -<see cref="HtmlFacts.SEPARATOR_FOR_ATTRIBUTE_NAME_AND_ATTRIBUTE_VALUE"/><br/>
        /// currentCharacterOut:<br/>
        /// -<see cref="HtmlFacts.ATTRIBUTE_VALUE_ENDING"/><br/>
        /// </summary>
        public static AttributeValueSyntax ParseAttributeValue(
            StringWalker stringWalker,
            TextEditorHtmlDiagnosticBag textEditorHtmlDiagnosticBag,
            InjectedLanguageDefinition? injectedLanguageDefinition)
        {
            var startingPositionIndex = stringWalker.PositionIndex;
            
            // Move to the first non-whitespace which follows the
            // HtmlFacts.SEPARATOR_FOR_ATTRIBUTE_NAME_AND_ATTRIBUTE_VALUE
            while (!stringWalker.IsEof)
            {
                _ = stringWalker.ReadCharacter();

                if (!WhitespaceFacts.ALL.Contains(stringWalker.CurrentCharacter))
                    break;
            }

            var foundOpenTagEnding = stringWalker.CheckForSubstringRange(
                HtmlFacts.OPEN_TAG_ENDING_OPTIONS,
                out _);

            if (!foundOpenTagEnding)
            {
                var beganWithAttributeValueStarting =
                    HtmlFacts.ATTRIBUTE_VALUE_STARTING == stringWalker.CurrentCharacter;
            
                while (!stringWalker.IsEof)
                {
                    _ = stringWalker.ReadCharacter();

                    if (!beganWithAttributeValueStarting && 
                        WhitespaceFacts.ALL.Contains(stringWalker.CurrentCharacter))
                    {
                        break;
                    }
                
                    if (stringWalker.CheckForSubstringRange(
                            HtmlFacts.OPEN_TAG_ENDING_OPTIONS, 
                            out _))
                    {
                        foundOpenTagEnding = true;
                        break; 
                    }
                
                    if (HtmlFacts.ATTRIBUTE_VALUE_ENDING == stringWalker.CurrentCharacter)
                        break;
                }
            }

            var endingIndexExclusive = stringWalker.PositionIndex;

            if (!foundOpenTagEnding)
                endingIndexExclusive++;

            var attributeValueTextSpan = new TextEditorTextSpan(
                startingPositionIndex,
                endingIndexExclusive,
                (byte)HtmlDecorationKind.AttributeValue);
            
            return new AttributeValueSyntax(attributeValueTextSpan);
        }
        
        public static CommentSyntax ParseComment(
            StringWalker stringWalker,
            TextEditorHtmlDiagnosticBag textEditorHtmlDiagnosticBag,
            InjectedLanguageDefinition? injectedLanguageDefinition)
        {
            var startingPositionIndex = stringWalker.PositionIndex;
            
            while (!stringWalker.IsEof)
            {
                _ = stringWalker.ReadCharacter();
                
                if (stringWalker.CheckForSubstring(HtmlFacts.COMMENT_TAG_ENDING))
                    break;
            }

            // Skip the remaining characters in the comment tag ending string
            _ = stringWalker.ReadRange(HtmlFacts.COMMENT_TAG_ENDING.Length - 1);
            
            var commentTagTextSpan = new TextEditorTextSpan(
                startingPositionIndex,
                stringWalker.PositionIndex + 1,
                (byte)HtmlDecorationKind.Comment);
            
            return new CommentSyntax(commentTagTextSpan);
        }
    }
}