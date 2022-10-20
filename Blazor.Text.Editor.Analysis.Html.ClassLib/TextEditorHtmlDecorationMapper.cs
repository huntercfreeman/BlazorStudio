using BlazorTextEditor.RazorLib.Decoration;

namespace Blazor.Text.Editor.Analysis.Html.ClassLib;

public class TextEditorHtmlDecorationMapper : IDecorationMapper
{
    public string Map(byte decorationByte)
    {
        var decoration = (HtmlDecorationKind)decorationByte;

        return decoration switch
        {
            HtmlDecorationKind.None => string.Empty,
            HtmlDecorationKind.Method => "bte_method",
            HtmlDecorationKind.Type => "bte_type",
            HtmlDecorationKind.Parameter => "bte_parameter",
            HtmlDecorationKind.StringLiteral => "bte_string-literal",
            HtmlDecorationKind.Keyword => "bte_keyword",
            HtmlDecorationKind.Comment => "bte_comment",
            _ => throw new ApplicationException(
                $"The {nameof(HtmlDecorationKind)}: {decoration} was not recognized.")
        };
    }
}