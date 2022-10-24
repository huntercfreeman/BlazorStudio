using System.Collections.Immutable;
using System.Text;
using Blazor.Text.Editor.Analysis.Html.ClassLib.Syntax;
using BlazorTextEditor.RazorLib.Lexing;

namespace Blazor.Text.Editor.Analysis.Html.ClassLib;

public static class HtmlSyntaxTree
{
    public static HtmlSyntaxUnit ParseText(
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
                    (byte)HtmlDecorationKind.None))
        };

        var textEditorHtmlDiagnosticBag = new TextEditorHtmlDiagnosticBag();

        rootTagSyntaxBuilder.ChildTagSyntaxes = HtmlSyntaxTreeStateMachine
                .ParseTagChildContent(
                    stringWalker,
                    textEditorHtmlDiagnosticBag,
                    injectedLanguageDefinition);

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
        /// <see cref="HtmlFacts.OPEN_TAG_BEGINNING"/>
        /// </summary>
        public static TagSyntax ParseTag(
            StringWalker stringWalker,
            TextEditorHtmlDiagnosticBag textEditorHtmlDiagnosticBag,
            InjectedLanguageDefinition? injectedLanguageDefinition)
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
                stringWalker.WhileNotEndOfFile(() =>
                {
                    var consumedCharacter = stringWalker.Consume();
                    
                    if (HtmlFacts.WHITESPACE.Contains(
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
                
                // Eager Skipping of Whitespace
                // results in the StringWalker going
                // 1 PositionIndex further than is wanted
                _ = stringWalker.Backtrack();
                
                if (stringWalker.CheckForSubstring(HtmlFacts.OPEN_TAG_WITH_CHILD_CONTENT_ENDING))
                {
                    // Ending of opening tag
                    tagBuilder.TagKind = TagKind.Opening;

                    // Skip the '>' character to set stringWalker at the first
                    // character of the child content
                    _ = stringWalker.Consume();
                    
                    tagBuilder.ChildTagSyntaxes = ParseTagChildContent(
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
                    _ = stringWalker.ConsumeRange(
                        HtmlFacts.OPEN_TAG_SELF_CLOSING_ENDING
                            .Length);
                    
                    // Ending of self-closing tag
                    tagBuilder.TagKind = TagKind.SelfClosing;

                    return tagBuilder.Build();
                }
                else if (stringWalker.CheckForSubstring(HtmlFacts.CLOSE_TAG_WITH_CHILD_CONTENT_BEGINNING))
                {
                    _ = stringWalker.ConsumeRange(
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
                            
                            _ = stringWalker.ConsumeRange(
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
                    
                    // TODO: Parse attributes - until then Consume to avoid infinite loop
                    _ = stringWalker.Consume();
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
            TextEditorHtmlDiagnosticBag textEditorHtmlDiagnosticBag,
            InjectedLanguageDefinition? injectedLanguageDefinition)
        {
            var startingPositionIndex = stringWalker.PositionIndex;

            var tagNameBuilder = new StringBuilder();
            
            stringWalker.WhileNotEndOfFile(() =>
            {
                if (stringWalker.CheckForSubstringRange(
                        HtmlFacts.TAG_NAME_STOP_DELIMITERS))
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
            TextEditorHtmlDiagnosticBag textEditorHtmlDiagnosticBag,
            InjectedLanguageDefinition? injectedLanguageDefinition)
        {
            var startingPositionIndex = stringWalker.PositionIndex;
            
            List<TagSyntax> tagSyntaxes = new();
            
            var textNodeBuilder = new StringBuilder();

            // Make a TagTextSyntax - HTML TextNode
            // if there was anything in the current builder
            void AddTextNode()
            {
                if (textNodeBuilder.Length <= 0) 
                    return;
                
                var tagTextSyntax = new TagTextSyntax(
                    ImmutableArray<AttributeTupleSyntax>.Empty,
                    ImmutableArray<TagSyntax>.Empty, 
                    textNodeBuilder.ToString());

                tagSyntaxes.Add(tagTextSyntax);
                textNodeBuilder.Clear();
            }
            
            stringWalker.WhileNotEndOfFile(() =>
            {
                if (stringWalker.CheckForSubstring(
                        HtmlFacts.CLOSE_TAG_WITH_CHILD_CONTENT_BEGINNING))
                {
                    return true;
                }
                else if (stringWalker.CurrentCharacter == 
                         HtmlFacts.OPEN_TAG_BEGINNING)
                {
                    // If there is text in textNodeBuilder
                    // add a new TextNode to the List of TagSyntax
                    AddTextNode();
                    
                    tagSyntaxes.Add(
                        ParseTag(
                            stringWalker,
                            textEditorHtmlDiagnosticBag,
                            injectedLanguageDefinition));
                    
                    return false;
                }
                else if (injectedLanguageDefinition is not null && stringWalker
                         .CheckForInjectedLanguageCodeBlockTag(injectedLanguageDefinition))
                {
                    // If there is text in textNodeBuilder
                    // add a new TextNode to the List of TagSyntax
                    AddTextNode();
                    
                    tagSyntaxes.AddRange(
                        ParseInjectedLanguageCodeBlock(
                            stringWalker,
                            textEditorHtmlDiagnosticBag,
                            injectedLanguageDefinition));
                    
                    return false;
                }
                else
                {
                    textNodeBuilder.Append(stringWalker.CurrentCharacter);
                    return false;
                }
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

            return tagSyntaxes;
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
                    ImmutableArray<TagSyntax>.Empty,
                    string.Empty,
                    new TextEditorTextSpan(
                        injectedLanguageFragmentSyntaxStartingPositionIndex,
                        stringWalker.PositionIndex + 1,
                        (byte)HtmlDecorationKind.InjectedLanguageFragment)));

            // Skip the "@" to give a .razor file example
            _ = stringWalker.Consume();

            injectedLanguageFragmentSyntaxes.AddRange(
                injectedLanguageDefinition.ParseInjectedLanguageFunc
                    .Invoke(
                        stringWalker,
                        textEditorHtmlDiagnosticBag,
                        injectedLanguageDefinition));

            return injectedLanguageFragmentSyntaxes;
        }
    }
}