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
            TagNameSyntax = new TagNameSyntax("document") 
        };

        var textEditorDiagnostics = new List<TextEditorDiagnostic>();

        if (stringWalker.Peek(0) == HtmlFacts.TAG_OPENING_CHARACTER)
        {
            rootTagSyntaxBuilder.ChildTagSyntaxes.Add(
                    HtmlSyntaxTreeStateMachine
                        .ParseTag(
                            stringWalker,
                            textEditorDiagnostics));
        }

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
        /// <see cref="HtmlFacts.TAG_OPENING_CHARACTER"/>
        /// </summary>
        public static TagSyntax ParseTag(
            StringWalker stringWalker, 
            List<TextEditorDiagnostic> textEditorDiagnostics)
        {
            var tagBuilder = new TagSyntax.TagSyntaxBuilder();

            // HtmlFacts.TAG_OPENING_CHARACTER
            _ = stringWalker.Consume();

            // Example: <!DOCTYPE html>
            if (stringWalker.Peek(0) == HtmlFacts.SPECIAL_HTML_TAG_CHARACTER)
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
                else if (stringWalker.CheckForSubstring(HtmlFacts.OPEN_TAG_ENDING_CHILD_CONTENT))
                {
                    // Ending of opening tag
                    tagBuilder.TagKind = TagKind.Opening;

                    // TODO: ParseTagChildContent
                    // ParseTagChildContent(tagBuilder.ChildTagSyntaxes);
                    
                    // TODO: ParseClosingTag
                }
                else if (stringWalker.CheckForSubstring(HtmlFacts.OPEN_TAG_ENDING_SELF_CLOSING))
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
        private static TagNameSyntax ParseTagName(
            StringWalker stringWalker,
            List<TextEditorDiagnostic> textEditorDiagnostics)
        {
            var captureLoopIteration = 0;
            
            var tagName = stringWalker.DoConsumeWhile(
                (builder, currentCharacter, loopIteration) =>
                {
                    captureLoopIteration = loopIteration;
                        
                    if (HtmlFacts.END_OF_TAG_NAME_DELIMITERS
                        .Contains(currentCharacter.ToString()))
                    {
                        return false;
                    }
                    
                    return true;
                });

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
                
                return new TagNameSyntax(tagName);
            }

            // The file was valid at this step and a TagName was read
            return new TagNameSyntax(tagName);
        }
    }
    
    private static void ParseTagChildContent(
        List<TagSyntax> tagBuilderChildTagSyntaxes,
        StringWalker stringWalker)
    {
        throw new NotImplementedException();
        // var textContent = stringWalker.DoConsumeWhile(
        //     (builder, currentCharacter, loopIteration) =>
        //     {
        //         
        //     });
    }
}
