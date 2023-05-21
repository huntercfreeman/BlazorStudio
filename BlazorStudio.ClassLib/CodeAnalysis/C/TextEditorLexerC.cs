using System.Collections.Immutable;
using BlazorCommon.RazorLib.Misc;
using BlazorStudio.ClassLib.CodeAnalysis.C.Syntax;
using BlazorTextEditor.RazorLib.Lexing;
using BlazorTextEditor.RazorLib.Model;

namespace BlazorStudio.ClassLib.CodeAnalysis.C;

public class TextEditorLexerC : ITextEditorLexer
{
    private readonly object _lexerLock = new object();

    public RenderStateKey ModelRenderStateKey { get; private set; } = RenderStateKey.Empty;
    public LexSession? RecentLexSession { get; private set; }

    public Task<ImmutableArray<TextEditorTextSpan>> Lex(
        string text,
        RenderStateKey modelRenderStateKey)
    {
        LexSession? lexSession;

        lock (_lexerLock)
        {
            lexSession = RecentLexSession;

            if (ModelRenderStateKey == modelRenderStateKey &&
                lexSession is not null)
            {
                return Task.FromResult(
                    lexSession.SyntaxTokens.Select(x => x.TextEditorTextSpan)
                    .ToImmutableArray());
            }
            
            ModelRenderStateKey = modelRenderStateKey;
        }

        lexSession = new LexSession(text);

        lexSession.Lex();

        RecentLexSession = lexSession;

        return Task.FromResult(
            lexSession.SyntaxTokens.Select(x => x.TextEditorTextSpan)
            .ToImmutableArray());
    }
}
