using System.Collections.Immutable;
using BlazorStudio.ClassLib.Parsing.C.SyntaxTokens;
using BlazorTextEditor.RazorLib.Analysis;

namespace BlazorStudio.ClassLib.Parsing.C;

public class Lexer
{
    private readonly StringWalker _stringWalker;
    private readonly List<ISyntaxToken> _syntaxTokens = new();
    
    public Lexer(string content)
    {
        _stringWalker = new StringWalker(content);
    }

    public ImmutableArray<ISyntaxToken> SyntaxTokens => _syntaxTokens.ToImmutableArray();

    /// <summary>
    /// General idea for this Lex method is to use a switch statement
    /// to invoke a method which returns the specific token.
    /// <br/><br/>
    /// The method also moves the position in the content forward.
    /// </summary>
    public void Lex()
    {
        while (!_stringWalker.IsEof)
        {
            switch (_stringWalker.CurrentCharacter)
            {
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                    var numericLiteralToken = ConsumeNumericLiteral();
                    _syntaxTokens.Add(numericLiteralToken);
                    break;
                default:
                    _ = _stringWalker.ReadCharacter();
                    break;
            }
        }
    }

    private NumericLiteralToken ConsumeNumericLiteral()
    {
        var entryPositionIndex = _stringWalker.PositionIndex;
        
        while (!_stringWalker.IsEof)
        {
            if (char.IsNumber(_stringWalker.CurrentCharacter))
            {
                _ = _stringWalker.ReadCharacter();
                continue;
            }

            break;
        }

        var textSpan = new BlazorStudioTextSpan(
            entryPositionIndex,
            _stringWalker.PositionIndex);
        
        return new NumericLiteralToken(textSpan);
    }
}