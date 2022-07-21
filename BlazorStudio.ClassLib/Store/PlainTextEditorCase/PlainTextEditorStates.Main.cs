using System.Collections.Immutable;
using Fluxor;

namespace BlazorStudio.ClassLib.Store.PlainTextEditorCase;

[FeatureState]
public partial record PlainTextEditorStates(ImmutableDictionary<PlainTextEditorKey, IPlainTextEditor> Map, 
    ImmutableArray<PlainTextEditorKey> Array)
{
    private PlainTextEditorStates() : this(new Dictionary<PlainTextEditorKey, IPlainTextEditor>().ToImmutableDictionary(),
        new PlainTextEditorKey[0].ToImmutableArray())
    {
        
    }
}
