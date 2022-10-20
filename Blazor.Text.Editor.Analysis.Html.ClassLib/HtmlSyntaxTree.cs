using System.Collections.Immutable;
using System.Text;
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
        /// of 0 be equal to
        /// <see cref="HtmlFacts.TAG_OPENING_CHARACTER"/>
        /// </summary>
        public static TagSyntax ParseTag(StringWalker stringWalker)
        {
            var tagBuilder = new TagSyntax.TagSyntaxBuilder();

            // HtmlFacts.TAG_OPENING_CHARACTER
            _ = stringWalker.Consume();

            // Example: <!DOCTYPE html>
            if (stringWalker.Peek(0) == HtmlFacts.SPECIAL_HTML_TAG_CHARACTER)
            {
                // HtmlFacts.SPECIAL_HTML_TAG_CHARACTER
                stringWalker.Consume();
                
                tagBuilder.TagNameSyntax = ParseTagName(stringWalker);

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

        /// <summary>
        /// Invocation of this method requires the
        /// stringWalker to have <see cref="StringWalker.Peek" />
        /// of 0 be equal to the first
        /// character that is part of the tag's name
        /// </summary>
        private static TagNameSyntax ParseTagName(StringWalker stringWalker)
        {
            var tagName = stringWalker.DoConsumeWhile(
                (builder, currentCharacter) =>
                {
                    if (HtmlFacts.END_OF_TAG_NAME_DELIMITERS
                        .Contains(currentCharacter.ToString()))
                    {
                        return false;
                    }
                    
                    return true;
                });

            return new TagNameSyntax(tagName);
        }
    }
}