using Blazor.Text.Editor.Analysis.Html.ClassLib;

namespace Blazor.Text.Editor.Analysis.Razor.ClassLib;

public static class RazorInjectedLanguageFacts
{
    public static readonly InjectedLanguageDefinition
        RazorInjectedLanguageDefinition = 
            new InjectedLanguageDefinition
            {
                InjectedLanguageCodeBlockTag = new []
                {
                    new InjectedLanguageCodeBlockTag
                    {
                        CodeBlockTag = "@",
                        CodeBlockOpening = "{",
                        CodeBlockClosing = "}"
                    },
                    new InjectedLanguageCodeBlockTag
                    {
                        CodeBlockTag = "@",
                        CodeBlockOpening = "(",
                        CodeBlockClosing = ")"
                    },
                    new InjectedLanguageCodeBlockTag
                    {
                        CodeBlockTag = "@",
                        // @if (myExpression) { <div>true</div> }
                        CodeBlockOpening = "TODO: any control keyword (like for, if, or switch)",
                        CodeBlockClosing = "}"
                    },
                    new InjectedLanguageCodeBlockTag
                    {
                        CodeBlockTag = "@",
                        CodeBlockOpening = "code{",
                        CodeBlockClosing = "}"
                    },
                    new InjectedLanguageCodeBlockTag
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