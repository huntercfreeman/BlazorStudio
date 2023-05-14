using System.Collections.Immutable;
using BlazorStudio.ClassLib.Parsing.C.SyntaxTokens;

namespace BlazorStudio.ClassLib.Parsing.C;

public class TokenWalker
{
    private readonly ImmutableArray<ISyntaxToken> _tokens;

    public TokenWalker(ImmutableArray<ISyntaxToken> tokens)
    {
        _tokens = tokens;
    }

    public ImmutableArray<ISyntaxToken> Tokens => _tokens;

    private int _index;

    public ISyntaxToken Peek(int offset)
    {
        var index = _index + offset;

        if (index >= _tokens.Length)
        {
            // Return the end of file token (the last token)
            return _tokens[_tokens.Length - 1];
        }

        return _tokens[index];
    }
}