using System.Collections.Immutable;
using BlazorStudio.ClassLib.Sequence;

namespace BlazorStudio.ClassLib.Store.PlainTextEditorCase;

public interface IPlainTextEditorRow
{
    public PlainTextEditorRowKey Key { get; } 
    public SequenceKey SequenceKey { get; }
    public ImmutableList<ITextToken> List { get; }
}
