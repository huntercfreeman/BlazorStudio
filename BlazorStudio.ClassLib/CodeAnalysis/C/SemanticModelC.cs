using BlazorStudio.ClassLib.CodeAnalysis.C.Syntax;
using BlazorStudio.ClassLib.CodeAnalysis.C.Syntax.SyntaxNodes;
using BlazorTextEditor.RazorLib.Diff;
using BlazorTextEditor.RazorLib.Lexing;
using BlazorTextEditor.RazorLib.Model;
using BlazorTextEditor.RazorLib.Semantics;
using System.Collections.Immutable;

namespace BlazorStudio.ClassLib.CodeAnalysis.C;

public class SemanticModelC : ISemanticModel
{
    private string? _text;
    private Lexer? _lexer;
    private Parser? _parser;
    private CompilationUnit? _compilationUnit;

    public ImmutableList<TextEditorTextSpan> TextEditorTextSpans { get; private set; } = ImmutableList<TextEditorTextSpan>.Empty;

    public SymbolDefinition? GoToDefinition(
        TextEditorModel model,
        TextEditorTextSpan textSpan)
    {
        _text = model.GetAllText();

        var identifier = textSpan.GetText(_text);

        _lexer = new Lexer(_text);
        _lexer.Lex();

        _parser = new Parser(
            _lexer.SyntaxTokens,
            _text,
            _lexer.Diagnostics);

        return null;
    }

    public void Parse(
        TextEditorModel model)
    {
        _text = model.GetAllText();

        _lexer = new Lexer(_text);
        _lexer.Lex();

        _parser = new Parser(
            _lexer.SyntaxTokens,
            _text,
            _lexer.Diagnostics);

        _compilationUnit = _parser.Parse();

        TextEditorTextSpans = _compilationUnit.Diagnostics.Select(x =>
        {
            var textEditorDecorationKind = x.BlazorStudioDiagnosticLevel switch
            {
                BlazorStudioDiagnosticLevel.Hint => TextEditorSemanticDecorationKind.DiagnosticHint,
                BlazorStudioDiagnosticLevel.Suggestion => TextEditorSemanticDecorationKind.DiagnosticSuggestion,
                BlazorStudioDiagnosticLevel.Warning => TextEditorSemanticDecorationKind.DiagnosticWarning,
                BlazorStudioDiagnosticLevel.Error => TextEditorSemanticDecorationKind.DiagnosticError,
                BlazorStudioDiagnosticLevel.Other => TextEditorSemanticDecorationKind.DiagnosticOther,
                _ => throw new NotImplementedException(),
            };

            return new TextEditorTextSpan(
                x.TextEditorTextSpan.StartingIndexInclusive,
                x.TextEditorTextSpan.EndingIndexExclusive,
                (byte)textEditorDecorationKind);
        }).ToImmutableList();
    }
}