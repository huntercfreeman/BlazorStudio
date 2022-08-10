using Microsoft.CodeAnalysis.CSharp;

namespace BlazorStudio.ClassLib.RoslynHelpers;

public static class SyntaxKindToCssStringConverter
{
    public static string Convert(SyntaxKind syntaxKind)
    {
        return syntaxKind switch
        {
            SyntaxKind.TypeParameter => "pte_plain-text-editor-text-token-display-type",
            _ => string.Empty
        };
    }
}