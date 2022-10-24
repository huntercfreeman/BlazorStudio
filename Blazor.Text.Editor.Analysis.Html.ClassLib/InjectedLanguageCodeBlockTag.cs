namespace Blazor.Text.Editor.Analysis.Html.ClassLib;

public class InjectedLanguageCodeBlockTag
{
    public string CodeBlockTag { get; set; }
    public string CodeBlockOpening { get; set; }
    public string CodeBlockClosing { get; set; }
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
    public Func<string, bool> DetermineClosingFunc { get; set; }
}