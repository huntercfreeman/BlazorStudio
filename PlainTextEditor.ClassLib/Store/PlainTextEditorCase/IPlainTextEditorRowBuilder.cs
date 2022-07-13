using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlainTextEditor.ClassLib.Store.PlainTextEditorCase;

public interface IPlainTextEditorRowBuilder
{
    public IPlainTextEditorRowBuilder Add(ITextToken token);
    public IPlainTextEditorRowBuilder Insert(int index, ITextToken token);
    public IPlainTextEditorRowBuilder Remove(TextTokenKey textTokenKey);
    public IPlainTextEditorRowBuilder ReplaceMapValue(ITextToken token);
    public IPlainTextEditorRow Build();
}
