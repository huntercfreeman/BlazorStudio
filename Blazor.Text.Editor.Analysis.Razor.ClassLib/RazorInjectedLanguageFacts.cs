using Blazor.Text.Editor.Analysis.Html.ClassLib;

namespace Blazor.Text.Editor.Analysis.Razor.ClassLib;

public static class RazorInjectedLanguageFacts
{
    public static readonly InjectedLanguageDefinition
        RazorInjectedLanguageDefinition = 
            new InjectedLanguageDefinition
            {
                InjectedLanguageCodeBlockTag = "@",
                InjectedLanguageCodeBlockTagEscaped = "@@",
                InjectedLanguageCodeBlocks = new []
                {
                    new InjectedLanguageCodeBlock
                    {
                        CodeBlockTag = "@",
                        CodeBlockOpening = "{",
                        CodeBlockClosing = "}"
                    },
                    new InjectedLanguageCodeBlock
                    {
                        CodeBlockTag = "@",
                        CodeBlockOpening = "(",
                        CodeBlockClosing = ")"
                    },
                    new InjectedLanguageCodeBlock
                    {
                        CodeBlockTag = "@",
                        // @if (myExpression) { <div>true</div> }
                        CodeBlockOpening = "TODO: any control keyword (like for, if, or switch)",
                        CodeBlockClosing = "}"
                    },
                    new InjectedLanguageCodeBlock
                    {
                        CodeBlockTag = "@",
                        CodeBlockOpening = "code{",
                        CodeBlockClosing = "}"
                    },
                    new InjectedLanguageCodeBlock
                    {
                        CodeBlockTag = "@",
                        DetermineClosingFunc = runningString =>
                        {
                            /*
                             * If the dictionary of C# variables finds
                             * a variable with the identifier of $"{runningString}"
                             * then return true we found a match
                             *
                             * But what about method calls is that allowed?
                             */
                            
                            if (true)
                            {
                                
                            }
                            
                            return true;
                        }
                    }
                }
            };
}