using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using PlainTextEditor.ClassLib.Store.PlainTextEditorCase;

namespace PlainTextEditor.RazorLib.PlainTextEditorCase;

public partial class TextTokenDisplay : ComponentBase
{
    [Parameter, EditorRequired]
    public ITextToken TextToken { get; set; } = null!;
}
