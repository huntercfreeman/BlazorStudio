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

            /*
         *
<!DOCTYPE html>
<html>
    <head>
        <!-- head definitions go here -->
    </head>
    <body>
        <!-- the content goes here -->
    </body>
</html>
         */
            
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
            int captureLoopIndex = 0;
            var captureTagNameBuilder = new StringBuilder();
            
            var tagName = stringWalker.DoConsumeWhile(
                (builder, currentCharacter, loopIndex) =>
                {
                    captureLoopIndex = loopIndex;
                    captureTagNameBuilder = builder;
                        
                    if (HtmlFacts.END_OF_TAG_NAME_DELIMITERS
                        .Contains(currentCharacter.ToString()))
                    {
                        return false;
                    }
                    
                    return true;
                });

            // The do while loop immediately
            // failed on the first loop
            if (captureLoopIndex == 0)
            {
                // Therefore fabricate a TagNameSyntax
                // with the invalid text as its tag name and report a diagnostic
                // so the rest of the file can still be parsed.

                textEditorDiagnostics.Add(new TextEditorDiagnostic(
                    DiagnosticLevel.Error,
                    $"The {nameof(TagNameSyntax)} of:" +
                    $" '{captureTagNameBuilder}'" +
                    $" is not valid.",
                    new TextEditorTextSpan(
                        stringWalker.Position - captureLoopIndex,
                        stringWalker.Position,
                        (byte)HtmlDecorationKind.Error)));
                
                return new TagNameSyntax(captureTagNameBuilder.ToString());
            }

            // The file was valid at this step and a TagName was read
            return new TagNameSyntax(tagName);
        }
    }
}
