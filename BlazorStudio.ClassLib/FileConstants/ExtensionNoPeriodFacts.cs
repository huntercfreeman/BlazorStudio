using BlazorTextEditor.RazorLib.Analysis.CSharp;
using BlazorTextEditor.RazorLib.Analysis.Html;
using BlazorTextEditor.RazorLib.Analysis.Html.Decoration;
using BlazorTextEditor.RazorLib.Analysis.Razor;
using BlazorTextEditor.RazorLib.Decoration;
using BlazorTextEditor.RazorLib.Lexing;

namespace BlazorStudio.ClassLib.FileConstants;

/// <summary>
/// The constants do not start with a period
/// </summary>
public static class ExtensionNoPeriodFacts
{
    public const string DOT_NET_SOLUTION = "sln";
    public const string C_SHARP_PROJECT = "csproj";
    public const string C_SHARP_CLASS = "cs";
    public const string CSHTML_CLASS = "cshtml";
    public const string RAZOR_MARKUP = "razor";
    public const string RAZOR_CODEBEHIND = "razor.cs";
    public const string JSON = "json";
    public const string HTML = "html";
    public const string XML = "xml";
    public const string CSS = "css";
    public const string JAVA_SCRIPT = "js";
    public const string TYPE_SCRIPT = "ts";
    public const string MARK_DOWN = "md";

    public static ILexer GetLexer(string extensionNoPeriod) => extensionNoPeriod switch
    {
        HTML => new TextEditorHtmlLexer(),
        XML => new TextEditorHtmlLexer(),
        C_SHARP_PROJECT => new TextEditorHtmlLexer(),
        C_SHARP_CLASS => new TextEditorCSharpLexer(),
        RAZOR_CODEBEHIND => new TextEditorCSharpLexer(), 
        RAZOR_MARKUP => new TextEditorRazorLexer(),
        _ => new TextEditorLexerDefault(),
    };

    public static IDecorationMapper GetDecorationMapper(string extensionNoPeriod) => extensionNoPeriod switch
    {
        HTML => new TextEditorHtmlDecorationMapper(),
        XML => new TextEditorHtmlDecorationMapper(),
        C_SHARP_PROJECT => new TextEditorHtmlDecorationMapper(),
        C_SHARP_CLASS => new TextEditorCSharpDecorationMapper(),
        RAZOR_CODEBEHIND => new TextEditorCSharpDecorationMapper(),
        RAZOR_MARKUP => new TextEditorHtmlDecorationMapper(),
        _ => new TextEditorDecorationMapperDefault(),
    };
}