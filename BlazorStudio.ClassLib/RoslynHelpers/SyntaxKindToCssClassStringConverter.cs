using Microsoft.CodeAnalysis.CSharp;

namespace BlazorStudio.ClassLib.RoslynHelpers;

public static class SyntaxKindToCssClassStringConverter
{
    public static string Convert(SyntaxKind syntaxKind)
    {
        return syntaxKind switch
        {
            _ => string.Empty
        };
    }
}

// _ => "pte_plain-text-editor-text-token-display-type",
// _ => "pte_plain-text-editor-text-token-display-method-declaration",
// _ => "pte_plain-text-editor-text-token-display-argument",
// _ => "pte_plain-text-editor-text-token-display-string-literal",
