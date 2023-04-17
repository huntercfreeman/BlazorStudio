namespace BlazorStudio.ClassLib.Parsing.C.SyntaxTokens;

public class SyntaxTokenHelper
{
    public static (ISyntaxToken syntaxToken, string text)[] GetTokenTextTuples(
        IEnumerable<ISyntaxToken> syntaxTokens,
        string sourceText)
    {
        return syntaxTokens
            .Select(x => 
                (x,
                    x.BlazorStudioTextSpan.GetText(sourceText)))
            .ToArray();
    }
}