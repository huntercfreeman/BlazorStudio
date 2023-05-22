﻿using BlazorTextEditor.RazorLib.Lexing;

namespace BlazorStudio.ClassLib.CodeAnalysis.C.Syntax.SyntaxTokens;

public class CloseBraceToken : ISyntaxToken
{
    public CloseBraceToken(
        TextEditorTextSpan textEditorTextSpan)
    {
        TextEditorTextSpan = textEditorTextSpan;
    }

    public TextEditorTextSpan TextEditorTextSpan { get; }
    public SyntaxKind SyntaxKind => SyntaxKind.CloseBraceToken;
}