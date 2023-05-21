using BlazorStudio.ClassLib.CodeAnalysis.C;
using BlazorStudio.ClassLib.Parsing.C;
using BlazorTextEditor.RazorLib.Analysis.C.SyntaxActors;
using BlazorTextEditor.RazorLib.Analysis.CSharp.SyntaxActors;
using BlazorTextEditor.RazorLib.Analysis.Css.Decoration;
using BlazorTextEditor.RazorLib.Analysis.Css.SyntaxActors;
using BlazorTextEditor.RazorLib.Analysis.FSharp.SyntaxActors;
using BlazorTextEditor.RazorLib.Analysis.GenericLexer.Decoration;
using BlazorTextEditor.RazorLib.Analysis.Html.Decoration;
using BlazorTextEditor.RazorLib.Analysis.Html.SyntaxActors;
using BlazorTextEditor.RazorLib.Analysis.JavaScript.SyntaxActors;
using BlazorTextEditor.RazorLib.Analysis.Json.Decoration;
using BlazorTextEditor.RazorLib.Analysis.Json.SyntaxActors;
using BlazorTextEditor.RazorLib.Analysis.Razor.SyntaxActors;
using BlazorTextEditor.RazorLib.Analysis.TypeScript.SyntaxActors;
using BlazorTextEditor.RazorLib.Decoration;
using BlazorTextEditor.RazorLib.Lexing;
using BlazorTextEditor.RazorLib.Semantics;

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
    public const string F_SHARP = "fs";
    public const string C = "c";
    public const string H = "h";
    public const string CPP = "cpp";
    public const string HPP = "hpp";

    public static ITextEditorLexer GetLexer(
        string extensionNoPeriod) => extensionNoPeriod switch
    {
        HTML => new TextEditorHtmlLexer(),
        XML => new TextEditorHtmlLexer(),
        C_SHARP_PROJECT => new TextEditorHtmlLexer(),
        C_SHARP_CLASS => new TextEditorCSharpLexer(),
        RAZOR_CODEBEHIND => new TextEditorCSharpLexer(), 
        RAZOR_MARKUP => new TextEditorRazorLexer(),
        CSHTML_CLASS => new TextEditorRazorLexer(),
        CSS => new TextEditorCssLexer(),
        JAVA_SCRIPT => new TextEditorJavaScriptLexer(),
        JSON => new TextEditorJsonLexer(),
        TYPE_SCRIPT => new TextEditorTypeScriptLexer(),
        F_SHARP => new TextEditorFSharpLexer(),
        C => new TextEditorLexerC(),
        H => new TextEditorCLexer(),
        CPP => new TextEditorCLexer(),
        HPP => new TextEditorCLexer(),
        _ => new TextEditorLexerDefault(),
    };

    public static IDecorationMapper GetDecorationMapper(
        string extensionNoPeriod) => extensionNoPeriod switch
    {
        HTML => new TextEditorHtmlDecorationMapper(),
        XML => new TextEditorHtmlDecorationMapper(),
        C_SHARP_PROJECT => new TextEditorHtmlDecorationMapper(),
        C_SHARP_CLASS => new GenericDecorationMapper(),
        RAZOR_CODEBEHIND => new GenericDecorationMapper(),
        RAZOR_MARKUP => new TextEditorHtmlDecorationMapper(),
        CSHTML_CLASS => new TextEditorHtmlDecorationMapper(),
        CSS => new TextEditorCssDecorationMapper(),
        JAVA_SCRIPT => new GenericDecorationMapper(),
        JSON => new TextEditorJsonDecorationMapper(),
        TYPE_SCRIPT => new GenericDecorationMapper(),
        F_SHARP => new GenericDecorationMapper(),
        C => new GenericDecorationMapper(),
        H => new GenericDecorationMapper(),
        CPP => new GenericDecorationMapper(),
        HPP => new GenericDecorationMapper(),
        _ => new TextEditorDecorationMapperDefault(),
    };
    
    public static ISemanticModel? GetSemanticModel(
        string extensionNoPeriod,
        ITextEditorLexer lexer) => extensionNoPeriod switch
    {
        HTML => null,
        XML => null,
        C_SHARP_PROJECT => null,
        C_SHARP_CLASS => null,
        RAZOR_CODEBEHIND => null,
        RAZOR_MARKUP => null,
        CSHTML_CLASS => null,
        CSS => null,
        JAVA_SCRIPT => null,
        JSON => null,
        TYPE_SCRIPT => null,
        F_SHARP => null,
        C => new SemanticModelC(),
        H => null,
        CPP => null,
        HPP => null,
        _ => null,
    };
}