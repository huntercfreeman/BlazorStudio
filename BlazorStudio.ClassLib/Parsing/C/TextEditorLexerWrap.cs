using System.Collections.Immutable;
using BlazorCommon.RazorLib.Misc;
using BlazorTextEditor.RazorLib.Lexing;

namespace BlazorStudio.ClassLib.Parsing.C;

public class TextEditorLexerWrap : ILexer
{
    public RenderStateKey TextEditorModelRenderStateKey { get; } = RenderStateKey.Empty;
    public Lexer Lexer { get; private set; }

    public Task<ImmutableArray<TextEditorTextSpan>> Lex(string text)
    {
        Lexer = new Lexer(text);

        Lexer.Lex();

        return Task.FromResult(
            Lexer.SyntaxTokens.Select(x => x.TextEditorTextSpan)
            .ToImmutableArray());
    }
}
