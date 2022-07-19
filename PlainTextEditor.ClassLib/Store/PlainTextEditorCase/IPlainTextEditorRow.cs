using PlainTextEditor.ClassLib.Sequence;
using System.Collections.Immutable;

namespace PlainTextEditor.ClassLib.Store.PlainTextEditorCase;

public interface IPlainTextEditorRow
{
    public PlainTextEditorRowKey Key { get; } 
    public SequenceKey SequenceKey { get; }
    public ImmutableList<ITextToken> List { get; }
}
