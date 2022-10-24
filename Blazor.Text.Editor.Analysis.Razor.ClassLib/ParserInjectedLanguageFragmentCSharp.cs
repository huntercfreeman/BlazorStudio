using System.Collections.Immutable;
using System.Text;
using Blazor.Text.Editor.Analysis.Html.ClassLib;
using Blazor.Text.Editor.Analysis.Html.ClassLib.Syntax;
using BlazorTextEditor.RazorLib.Lexing;

namespace Blazor.Text.Editor.Analysis.Razor.ClassLib;

public static class ParserInjectedLanguageFragmentCSharp
{
    public static List<TagSyntax> ParseInjectedLanguageFragment(
        StringWalker stringWalker,
        TextEditorHtmlDiagnosticBag textEditorHtmlDiagnosticBag,
        InjectedLanguageDefinition injectedLanguageDefinition)
    {
        var injectedLanguageFragmentSyntaxes = new List<TagSyntax>();

        var codeBlockOrExpressionStartingPositionIndex = stringWalker.PositionIndex;

        var foundCodeBlock = false;
        
        stringWalker.WhileNotEndOfFile(() =>
        {
            // Try find matching code block opening syntax
            foreach (var codeBlock in injectedLanguageDefinition.InjectedLanguageCodeBlocks)
            {
                if (stringWalker.CheckForSubstring(
                        codeBlock.CodeBlockOpening))
                {
                    foundCodeBlock = true;
                    
                    // Track text span of the "{" character (example in .razor files)
                    // also will track the word "code"
                    //
                    // Given these follow an "@" character (example in .razor files)
                    injectedLanguageFragmentSyntaxes.Add(
                        new InjectedLanguageFragmentSyntax(
                            ImmutableArray<TagSyntax>.Empty,
                            string.Empty,
                            new TextEditorTextSpan(
                                codeBlockOrExpressionStartingPositionIndex,
                                stringWalker.PositionIndex +
                                codeBlock.CodeBlockOpening.Length,
                                (byte)HtmlDecorationKind.InjectedLanguageFragment)));

                    _ = stringWalker
                        .ConsumeRange(codeBlock.CodeBlockOpening.Length);

                    // > 0 means more opening brackets than closings
                    // once 0 is met then we've found the closing bracket
                    // start findMatchCounter = 1 because we're starting
                    // at the opening of the injected language code block
                    // and want to find the closing bracket of that given code block.
                    var findMatchCounter = 1;
                    
                    // While !EOF continue checking for the respective closing syntax
                    // for the previously matched code block opening syntax.
                    stringWalker.WhileNotEndOfFile(() =>
                    {
                        if (stringWalker.CheckForSubstring(
                                codeBlock.CodeBlockOpening) ||
                            // this or is hacky but @code{ ... } is messing things up
                            // and I am going to do this short term and come back.
                            stringWalker.CheckForSubstring("{"))
                        {
                            findMatchCounter++;
                        }
                        else if (stringWalker.CheckForSubstring(
                                     codeBlock.CodeBlockClosing))
                        {
                            findMatchCounter--;
                        }

                        if (findMatchCounter == 0)
                        {
                            // Track text span of the "}" character (example in .razor files)
                            // also will track the ending ")" character given it is the
                            // end of a code block.
                            injectedLanguageFragmentSyntaxes.Add(
                                new InjectedLanguageFragmentSyntax(
                                    ImmutableArray<TagSyntax>.Empty,
                                    string.Empty,
                                    new TextEditorTextSpan(
                                        stringWalker.PositionIndex,
                                        stringWalker.PositionIndex +
                                        codeBlock.CodeBlockClosing.Length,
                                        (byte)HtmlDecorationKind.InjectedLanguageFragment)));
                            
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    });

                    return true;
                }
            }

            return true;
        });

        if (!foundCodeBlock)
        {
            var expressionBuilder = new StringBuilder();

            stringWalker.WhileNotEndOfFile(() =>
            {
                // There was no matching code block opening syntax.
                // Therefore assume an expression syntax and allow the
                // InjectedLanguageDefinition access to a continually appended to
                // StringBuilder so the InjectedLanguageDefinition can determine
                // when expression ends.
                //
                // (Perhaps it matches a known variable name).
                //
                // (Perhaps there is an unmatched variable name however
                // there is a non valid character in the variable name therefore
                // match what was expected to allow for parsing the remainder of the file).

                expressionBuilder.Append(stringWalker.CurrentCharacter);

                return true;
            });
        }

        return injectedLanguageFragmentSyntaxes;
    }
}