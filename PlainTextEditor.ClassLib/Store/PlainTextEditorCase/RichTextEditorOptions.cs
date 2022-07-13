using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlainTextEditor.ClassLib.Store.PlainTextEditorCase;

public record RichTextEditorOptions
{
    public int FontSizeInPixels { get; init; } = 18;
}
