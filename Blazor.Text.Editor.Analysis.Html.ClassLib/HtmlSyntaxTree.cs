using System.Collections.Immutable;
using System.Text;
using Blazor.Text.Editor.Analysis.Html.ClassLib.Syntax;
using Blazor.Text.Editor.Analysis.Shared;
using BlazorTextEditor.RazorLib.Lexing;

namespace Blazor.Text.Editor.Analysis.Html.ClassLib;

public static class HtmlSyntaxTree
{
    public static HtmlSyntaxUnit ParseText(string content)
    {
        var stringWalker = new StringWalker(content);

        var rootTagSyntaxBuilder = new TagSyntax.TagSyntaxBuilder
        {
            TagNameSyntax = new TagNameSyntax(
                "document", 
                new TextEditorTextSpan(
                    0,
                    0,
                    (byte)HtmlDecorationKind.None))
        };

        var textEditorHtmlDiagnosticBag = new TextEditorHtmlDiagnosticBag();

        rootTagSyntaxBuilder.ChildTagSyntaxes = HtmlSyntaxTreeStateMachine
                .ParseTagChildContent(
                    stringWalker,
                    textEditorHtmlDiagnosticBag);

        var htmlSyntaxUnitBuilder = new HtmlSyntaxUnit.HtmlSyntaxUnitBuilder
        {
            RootTagSyntax = rootTagSyntaxBuilder.Build(),
            TextEditorHtmlDiagnosticBag = textEditorHtmlDiagnosticBag
        };

        return htmlSyntaxUnitBuilder.Build();
    }

    private static class HtmlSyntaxTreeStateMachine
    {
        /// <summary>
        /// Invocation of this method requires the
        /// stringWalker to have <see cref="StringWalker.Peek" />
        /// of 0 be equal to
        /// <see cref="HtmlFacts.START_OPEN_TAG"/>
        /// </summary>
        public static TagSyntax ParseTag(
            StringWalker stringWalker,
            TextEditorHtmlDiagnosticBag textEditorHtmlDiagnosticBag)
        {
            var startingPositionIndex = stringWalker.PositionIndex;
            
            var tagBuilder = new TagSyntax.TagSyntaxBuilder();

            // HtmlFacts.TAG_OPENING_CHARACTER
            _ = stringWalker.Consume();

            // Example: <!DOCTYPE html>
            if (stringWalker.Peek(0) == HtmlFacts.SPECIAL_HTML_TAG)
            {
                // HtmlFacts.SPECIAL_HTML_TAG_CHARACTER
                stringWalker.Consume();

                tagBuilder.HasSpecialHtmlCharacter = true;
            }

            tagBuilder.TagNameSyntax = ParseTagName(
                stringWalker,
                textEditorHtmlDiagnosticBag);

            // Get all html attributes
            // break when see End Of File or
            // closing of the tag
            while (true)
            {
                // Skip Whitespace
                stringWalker.WhileNotEndOfFile(() =>
                {
                    var consumedCharacter = stringWalker.Consume();
                    
                    if (HtmlFacts.HTML_WHITESPACE.Contains(
                            consumedCharacter.ToString()))
                    {
                        return false;
                    }

                    return true;
                });

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
                
                if (stringWalker.CheckForSubstring(HtmlFacts.END_OPEN_TAG_WITH_CHILD_CONTENT))
                {
                    // Ending of opening tag
                    tagBuilder.TagKind = TagKind.Opening;

                    // Skip the '>' character to set stringWalker at the first
                    // character of the child content
                    _ = stringWalker.Consume();
                    
                    tagBuilder.ChildTagSyntaxes = ParseTagChildContent(
                        stringWalker,
                        textEditorHtmlDiagnosticBag);

                    // At the closing tag now so check that the closing tag
                    // name matches the opening tag.
                    //
                    // An opening tag of
                    //     <div>
                    // Should have a matching closing tag of
                    //     </div>
                    
                    // TODO: check that the closing tag name matches the opening tag
                }
                else if (stringWalker.CheckForSubstring(HtmlFacts.END_OPEN_TAG_SELF_CLOSING))
                {
                    // Ending of self-closing tag
                    tagBuilder.TagKind = TagKind.SelfClosing;

                    return tagBuilder.Build();
                }
                else if (stringWalker.CheckForSubstring(HtmlFacts.START_CLOSE_TAG_WITH_CHILD_CONTENT))
                {
                    return tagBuilder.Build(); 
                }
                else
                {
                    // Attribute
                }
            }
        }

        /// <summary>
        /// Invocation of this method requires the
        /// stringWalker to have <see cref="StringWalker.Peek" />
        /// of 0 be equal to the first
        /// character that is part of the tag's name
        /// </summary>
        public static TagNameSyntax ParseTagName(
            StringWalker stringWalker,
            TextEditorHtmlDiagnosticBag textEditorHtmlDiagnosticBag)
        {
            var startingPositionIndex = stringWalker.PositionIndex;

            var tagNameBuilder = new StringBuilder();
            
            stringWalker.WhileNotEndOfFile(() =>
            {
                if (stringWalker.CheckForSubstringRange(
                        HtmlFacts.END_TAG_NAME_DELIMITERS))
                {
                    return true;
                }

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

        public static List<TagSyntax> ParseTagChildContent(
            StringWalker stringWalker,
            TextEditorHtmlDiagnosticBag textEditorHtmlDiagnosticBag)
        {
            List<TagSyntax> tagSyntaxes = new();

            _ = stringWalker.DoConsumeWhile(
                (builder, currentCharacter, loopIteration) =>
                {
                    // Make a TagTextSyntax - HTML TextNode
                    // if there was anything in the current builder
                    void AddCurrentTagTextSyntax()
                    {
                        if (builder.Length > 0)
                        {
                            var tagTextSyntax = new TagTextSyntax(
                                ImmutableArray<AttributeTupleSyntax>.Empty,
                                ImmutableArray<TagSyntax>.Empty, 
                                builder.ToString());

                            tagSyntaxes.Add(tagTextSyntax);
                            builder.Clear();
                        }
                    }
                    
                    // While we do not see:
                    //     -The closing tag of the parent HTML element
                    //     -The end of the file
                    // if (currentCharacter is the start of an HTML tag opening):
                    //     -If builder.Length > 0 then create a TagTextSyntax with
                    //         the value of builder.ToString() and add it to the List
                    //         of TagSyntax that will be returned from this method.
                    //     -ParseTag() and Add the result to the List of TagSyntax
                    //         that will be returned from this method.
                    // else:
                    //     -Append to StringBuilder all the text found
                    //         and treat the text found as the value of
                    //         an HTML TextNode

                    if (stringWalker.CheckForSubstring(HtmlFacts.START_CLOSE_TAG_WITH_CHILD_CONTENT) ||
                        ParserFacts.END_OF_FILE == currentCharacter)
                    {
                        // End of tag's child content
                        // so stop do while loop
                        
                        AddCurrentTagTextSyntax();
                        
                        return false;
                    }
                    else if (HtmlFacts.START_OPEN_TAG == currentCharacter)
                    {
                        AddCurrentTagTextSyntax();

                        stringWalker.Backtrack();

                        tagSyntaxes.Add(
                            ParseTag(
                                stringWalker,
                                textEditorHtmlDiagnosticBag));
                    }

                    return true;
                });

            return tagSyntaxes;
        }
    }
}