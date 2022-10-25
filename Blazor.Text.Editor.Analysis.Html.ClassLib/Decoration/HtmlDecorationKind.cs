namespace Blazor.Text.Editor.Analysis.Html.ClassLib.Decoration;

public enum HtmlDecorationKind
{
    None,
    AttributeName,
    AttributeValue,
    Comment,
    CustomTagName,
    EntityReference,
    HtmlCode,
    InjectedLanguageFragment,
    TagName,
    Tag,
    Error,
    InjectedLanguageCodeBlock,
    InjectedLanguageCodeBlockTag,
    InjectedLanguageKeyword,
    InjectedLanguageTagHelperAttribute,
    InjectedLanguageTagHelperElement,
    InjectedLanguageMethod,
    InjectedLanguageVariable,
    InjectedLanguageType,
    InjectedLanguageStringLiteral
}