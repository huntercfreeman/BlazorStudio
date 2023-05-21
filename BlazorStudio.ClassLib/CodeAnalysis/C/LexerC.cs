using System.Collections.Immutable;
using BlazorCommon.RazorLib.Misc;
using BlazorStudio.ClassLib.CodeAnalysis.C.Syntax;
using BlazorTextEditor.RazorLib.Lexing;

namespace BlazorStudio.ClassLib.CodeAnalysis.C;

public class LexerC : ILexer
{
    public RenderStateKey TextEditorModelRenderStateKey { get; } = RenderStateKey.Empty;
    public Lexer? Lexer { get; private set; }

    public Task<ImmutableArray<TextEditorTextSpan>> Lex(string text)
    {
        var lexer = new Lexer(text);

        lexer.Lex();

        Lexer = lexer;

        return Task.FromResult(
            lexer.SyntaxTokens.Select(x => x.TextEditorTextSpan)
            .ToImmutableArray());
    }
}
