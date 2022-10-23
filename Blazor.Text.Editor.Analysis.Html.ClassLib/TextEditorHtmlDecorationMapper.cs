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
            HtmlDecorationKind.AttributeName => "bte_attribute-name",
            HtmlDecorationKind.AttributeValue => "bte_attribute-value",
            HtmlDecorationKind.Comment => "bte_comment",
            HtmlDecorationKind.CustomTagName => "bte_custom-tag-name",
            HtmlDecorationKind.EntityReference => "bte_entity-reference",
            HtmlDecorationKind.HtmlCode => "bte_html-code",
            HtmlDecorationKind.InjectedLanguageFragment => "bte_injected-language-fragment",
            HtmlDecorationKind.TagName => "bte_tag-name",
            HtmlDecorationKind.Tag => "bte_tag",
            HtmlDecorationKind.Error => "bte_error",
            _ => throw new ApplicationException(
                $"The {nameof(HtmlDecorationKind)}: {decoration} was not recognized.")
        };
    }
}