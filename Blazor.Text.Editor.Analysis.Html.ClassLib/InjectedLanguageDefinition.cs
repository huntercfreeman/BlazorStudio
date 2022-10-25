using Blazor.Text.Editor.Analysis.Html.ClassLib.SyntaxItems;
using Blazor.Text.Editor.Analysis.Shared;

namespace Blazor.Text.Editor.Analysis.Html.ClassLib;

public class InjectedLanguageDefinition
{
    public string InjectedLanguageCodeBlockTag { get; set; }
    public string InjectedLanguageCodeBlockTagEscaped { get; set; }
    public InjectedLanguageCodeBlock[] InjectedLanguageCodeBlocks { get; set; }
    /// <summary>
    /// Assume following the <see cref="CodeBlockTag"/> there is
    /// a single expression and that the func will notify the HTML
    /// parser when the razor expression is done.
    /// <br/><br/>
    /// This case seems a bit weird because if I have:
    ///     "&lt;div&gt;@MyExpressionbuthereissometext&lt;/div&gt;"
    /// <br/><br/>
    /// then @MyExpression is the expression
    /// however "buthereissometext" comes immediately after
    /// without a space deliminating the two lexical tokens.
    /// <br/><br/>
    /// Is it the case that this is even valid in a .razor file?
    /// It is 4:16 AM and I am very tired but leaving this comment
    /// for myself for the future.
    /// </summary>
    public Func<StringWalker, TextEditorHtmlDiagnosticBag, InjectedLanguageDefinition, List<TagSyntax>> ParseInjectedLanguageFunc { get; set; }
}