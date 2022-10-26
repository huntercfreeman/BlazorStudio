using System.Collections.Immutable;
using System.Text;
using Blazor.Text.Editor.Analysis.CSharp.ClassLib;
using Blazor.Text.Editor.Analysis.Html.ClassLib;
using Blazor.Text.Editor.Analysis.Html.ClassLib.Decoration;
using Blazor.Text.Editor.Analysis.Html.ClassLib.InjectLanguage;
using Blazor.Text.Editor.Analysis.Html.ClassLib.SyntaxItems;
using Blazor.Text.Editor.Analysis.Shared;
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

                    var injectedLanguageOffsetPositionIndex = stringWalker.PositionIndex;

                    var injectedLanguageBuilder = new StringBuilder();

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
                        injectedLanguageBuilder.Append(stringWalker.CurrentCharacter);

                        if (stringWalker.CheckForSubstring(
                                codeBlock.CodeBlockOpening) ||
                            // this or is hacky but @code{ ... } is messing things up
                            // and I am going to do this short term and come back.
                            stringWalker.CheckForSubstring("{"))
                            findMatchCounter++;
                        else if (stringWalker.CheckForSubstring(
                                     codeBlock.CodeBlockClosing))
                            findMatchCounter--;

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

                        return false;
                    });

                    var lexer = new TextEditorCSharpLexer();

                    var classTemplateOpening = "public class Aaa{";

                    var injectedLanguageString = classTemplateOpening +
                                                 injectedLanguageBuilder.ToString();

                    var lexedInjectedLanguage = lexer.Lex(
                            injectedLanguageString)
                        .Result;

                    foreach (var lexedTokenTextSpan in lexedInjectedLanguage)
                    {
                        var startingIndexInclusive = lexedTokenTextSpan.StartingIndexInclusive +
                                                     injectedLanguageOffsetPositionIndex -
                                                     classTemplateOpening.Length;

                        var endingIndexExclusive = lexedTokenTextSpan.EndingIndexExclusive +
                                                   injectedLanguageOffsetPositionIndex -
                                                   classTemplateOpening.Length;

                        // startingIndexInclusive < 0 means it was part of the class
                        // template that was prepended so roslyn would recognize methods
                        if (lexedTokenTextSpan.StartingIndexInclusive - classTemplateOpening.Length
                            < 0)
                            continue;

                        var cSharpDecorationKind = (CSharpDecorationKind)lexedTokenTextSpan.DecorationByte;

                        switch (cSharpDecorationKind)
                        {
                            case CSharpDecorationKind.None:
                                break;
                            case CSharpDecorationKind.Method:
                                var razorMethodTextSpan = lexedTokenTextSpan with
                                {
                                    DecorationByte = (byte)HtmlDecorationKind.InjectedLanguageMethod,
                                    StartingIndexInclusive = startingIndexInclusive,
                                    EndingIndexExclusive = endingIndexExclusive,
                                };

                                injectedLanguageFragmentSyntaxes.Add(new InjectedLanguageFragmentSyntax(
                                    ImmutableArray<TagSyntax>.Empty,
                                    stringWalker.GetText(razorMethodTextSpan),
                                    razorMethodTextSpan));

                                break;
                            case CSharpDecorationKind.Type:
                                var razorTypeTextSpan = lexedTokenTextSpan with
                                {
                                    DecorationByte = (byte)HtmlDecorationKind.InjectedLanguageType,
                                    StartingIndexInclusive = startingIndexInclusive,
                                    EndingIndexExclusive = endingIndexExclusive,
                                };

                                injectedLanguageFragmentSyntaxes.Add(new InjectedLanguageFragmentSyntax(
                                    ImmutableArray<TagSyntax>.Empty,
                                    stringWalker.GetText(razorTypeTextSpan),
                                    razorTypeTextSpan));

                                break;
                            case CSharpDecorationKind.Parameter:
                                var razorVariableTextSpan = lexedTokenTextSpan with
                                {
                                    DecorationByte = (byte)HtmlDecorationKind.InjectedLanguageVariable,
                                    StartingIndexInclusive = startingIndexInclusive,
                                    EndingIndexExclusive = endingIndexExclusive,
                                };

                                injectedLanguageFragmentSyntaxes.Add(new InjectedLanguageFragmentSyntax(
                                    ImmutableArray<TagSyntax>.Empty,
                                    stringWalker.GetText(razorVariableTextSpan),
                                    razorVariableTextSpan));

                                break;
                            case CSharpDecorationKind.StringLiteral:
                                var razorStringLiteralTextSpan = lexedTokenTextSpan with
                                {
                                    DecorationByte = (byte)HtmlDecorationKind.InjectedLanguageStringLiteral,
                                    StartingIndexInclusive = startingIndexInclusive,
                                    EndingIndexExclusive = endingIndexExclusive,
                                };

                                injectedLanguageFragmentSyntaxes.Add(new InjectedLanguageFragmentSyntax(
                                    ImmutableArray<TagSyntax>.Empty,
                                    stringWalker.GetText(razorStringLiteralTextSpan),
                                    razorStringLiteralTextSpan));

                                break;
                            case CSharpDecorationKind.Keyword:
                                var razorKeywordTextSpan = lexedTokenTextSpan with
                                {
                                    DecorationByte = (byte)HtmlDecorationKind.InjectedLanguageKeyword,
                                    StartingIndexInclusive = startingIndexInclusive,
                                    EndingIndexExclusive = endingIndexExclusive,
                                };

                                injectedLanguageFragmentSyntaxes.Add(new InjectedLanguageFragmentSyntax(
                                    ImmutableArray<TagSyntax>.Empty,
                                    stringWalker.GetText(razorKeywordTextSpan),
                                    razorKeywordTextSpan));

                                break;
                            case CSharpDecorationKind.Comment:
                                var razorCommentTextSpan = lexedTokenTextSpan with
                                {
                                    DecorationByte = (byte)HtmlDecorationKind.Comment,
                                    StartingIndexInclusive = startingIndexInclusive,
                                    EndingIndexExclusive = endingIndexExclusive,
                                };

                                injectedLanguageFragmentSyntaxes.Add(new InjectedLanguageFragmentSyntax(
                                    ImmutableArray<TagSyntax>.Empty,
                                    stringWalker.GetText(razorCommentTextSpan),
                                    razorCommentTextSpan));

                                break;
                            default:
                                break;
                        }
                    }

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