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

        var textEditorDiagnostics = new List<TextEditorDiagnostic>();

        rootTagSyntaxBuilder.ChildTagSyntaxes = HtmlSyntaxTreeStateMachine
                .ParseTagChildContent(
                    stringWalker,
                    textEditorDiagnostics);

        var htmlSyntaxUnitBuilder = new HtmlSyntaxUnit.HtmlSyntaxUnitBuilder
        {
            RootTagSyntax = rootTagSyntaxBuilder.Build()
        };

        htmlSyntaxUnitBuilder.TextEditorDiagnostics
            .AddRange(textEditorDiagnostics);

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
            List<TextEditorDiagnostic> textEditorDiagnostics)
        {
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
                textEditorDiagnostics);

            // Get all html attributes
            // break when see End Of File or
            // closing of the tag
            while (true)
            {
                var captureLoopIteration = 0;

                // Skip all the whitespace before
                // the next non-whitespace character
                var skippedWhitespace = stringWalker.DoConsumeWhile(
                    (builder, currentCharacter, loopIteration) =>
                    {
                        captureLoopIteration = loopIteration;

                        if (HtmlFacts.HTML_WHITESPACE
                            .Contains(currentCharacter.ToString()))
                        {
                            return true;
                        }

                        return false;
                    });

                // Eager consumption results in the
                // need to Backtrack() by 1 character
                var backtrackCharacter = stringWalker.Backtrack();

                // End Of File is unexpected at this point so return a diagnostic.
                if (skippedWhitespace.EndsWith(ParserFacts.END_OF_FILE))
                {
                    textEditorDiagnostics.Add(new TextEditorDiagnostic(
                        DiagnosticLevel.Error,
                        $"'End of file' was unexpected." +
                        $" Wanted an ['attribute' OR 'closing tag'].",
                        new TextEditorTextSpan(
                            stringWalker.Position - captureLoopIteration,
                            stringWalker.Position,
                            (byte)HtmlDecorationKind.Error)));

                    return tagBuilder.Build();
                }
                else if (stringWalker.CheckForSubstring(HtmlFacts.END_OPEN_TAG_WITH_CHILD_CONTENT))
                {
                    // Ending of opening tag
                    tagBuilder.TagKind = TagKind.Opening;

                    tagBuilder.ChildTagSyntaxes = ParseTagChildContent(
                        stringWalker,
                        textEditorDiagnostics);

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
                else
                {
                    // Attribute
                }
            }

            /*
             *
             * <div class="bstudio_navbar">
             *     <div>Navbar</div>
             * </div>
            */

            return tagBuilder.Build();
        }

        /// <summary>
        /// Invocation of this method requires the
        /// stringWalker to have <see cref="StringWalker.Peek" />
        /// of 0 be equal to the first
        /// character that is part of the tag's name
        /// </summary>
        public static TagNameSyntax ParseTagName(
            StringWalker stringWalker,
            List<TextEditorDiagnostic> textEditorDiagnostics)
        {
            var captureLoopIteration = 0;

            var startingPositionIndex = stringWalker.Position;
            
            var tagName = stringWalker.DoConsumeWhile(
                (builder, currentCharacter, loopIteration) =>
                {
                    captureLoopIteration = loopIteration;

                    if (HtmlFacts.END_TAG_NAME_DELIMITERS
                        .Contains(currentCharacter.ToString()))
                    {
                        return false;
                    }

                    return true;
                });
            
            var endingPositionIndex = stringWalker.Position;

            // The do while loop immediately
            // failed on the first loop
            if (captureLoopIteration == 0)
            {
                // Therefore fabricate a TagNameSyntax
                // with the invalid text as its tag name and report a diagnostic
                // so the rest of the file can still be parsed.

                textEditorDiagnostics.Add(new TextEditorDiagnostic(
                    DiagnosticLevel.Error,
                    $"The {nameof(TagNameSyntax)} of:" +
                    $" '{tagName}'" +
                    $" is not valid.",
                    new TextEditorTextSpan(
                        stringWalker.Position - captureLoopIteration,
                        stringWalker.Position,
                        (byte)HtmlDecorationKind.Error)));

                return new TagNameSyntax(
                    tagName,
                    new TextEditorTextSpan(
                        startingPositionIndex,
                        endingPositionIndex,
                        (byte)HtmlDecorationKind.TagName));
            }

            // The file was valid at this step and a TagName was read
            return new TagNameSyntax(
                tagName,
                new TextEditorTextSpan(
                    startingPositionIndex,
                    endingPositionIndex,
                    (byte)HtmlDecorationKind.TagName));
        }

        public static List<TagSyntax> ParseTagChildContent(
            StringWalker stringWalker,
            List<TextEditorDiagnostic> textEditorDiagnostics)
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

                        tagSyntaxes.Add(
                            ParseTag(
                                stringWalker,
                                textEditorDiagnostics));
                    }

                    return true;
                });

            return tagSyntaxes;
        }
    }
}