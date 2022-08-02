﻿using BlazorStudio.ClassLib.Store.PlainTextEditorCase;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.PlainTextEditorCase;

public partial class PlainTextEditorFooter : ComponentBase
{
    [Parameter]
    public IPlainTextEditor PlainTextEditor { get; set; } = null!;

    private MarkupString GetIsCarriageReturnNewLine => (MarkupString) (PlainTextEditor.UseCarriageReturnNewLine
        ? "CR&nbsp;LF"
        : "LF");
}