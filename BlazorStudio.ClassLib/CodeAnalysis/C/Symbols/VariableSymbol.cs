using BlazorStudio.ClassLib.CodeAnalysis.C.Syntax;
using BlazorTextEditor.RazorLib.Lexing;

namespace BlazorStudio.ClassLib.CodeAnalysis.C.Symbols;

public class VariableSymbol : ISymbol
{
    public VariableSymbol(
        TextEditorTextSpan textSpan)
    {
        TextSpan = textSpan;
    }

    public TextEditorTextSpan TextSpan { get; }
    public SyntaxKind SyntaxKind => SyntaxKind.VariableSymbol;
}

