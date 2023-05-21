using BlazorTextEditor.RazorLib.Lexing;

namespace BlazorStudio.ClassLib.CodeAnalysis.C.Syntax.SyntaxTokens;

public interface ISyntaxToken : ISyntax
{
    public TextEditorTextSpan TextEditorTextSpan { get; }
}