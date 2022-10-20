using System.Collections.Immutable;
using Blazor.Text.Editor.Analysis.Html.ClassLib.Syntax;
using Blazor.Text.Editor.Analysis.Shared;

namespace Blazor.Text.Editor.Analysis.Html.ClassLib;

public static class HtmlSyntaxTree
{
    public static TagSyntax ParseText(string content)
    {
        var stringWalker = new StringWalker(content);

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

        var root = new TagSyntax.TagSyntaxBuilder
        {
            TagNameSyntax = new TagNameSyntax("document") 
        };

        if (stringWalker.Peek(0) == HtmlFacts.TAG_OPENING_CHARACTER)
        {
            root.ChildTagSyntaxes.Add(
                    HtmlSyntaxTreeStateMachine
                        .ParseTag(stringWalker));
        }

        return root.Build();
    }

    private static class HtmlSyntaxTreeStateMachine
    {
        /// <summary>
        /// Invocation of this method requires the
        /// stringWalker to have <see cref="StringWalker.Peek" />
        /// of 0 to be equal to
        /// <see cref="HtmlFacts.TAG_OPENING_CHARACTER"/>
        /// </summary>
        public static TagSyntax ParseTag(StringWalker stringWalker)
        {
            var tagBuilder = new TagSyntax.TagSyntaxBuilder();

            // tagOpeningCharacter
            _ = stringWalker.Consume();

            // <!DOCTYPE html>
            if (stringWalker.Peek(0) == HtmlFacts.SPECIAL_HTML_TAG_CHARACTER)
            {

                var name = parseTagName;

            }
            
            
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
    }
}