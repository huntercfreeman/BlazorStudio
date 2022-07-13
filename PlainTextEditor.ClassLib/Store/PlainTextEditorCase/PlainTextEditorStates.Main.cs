using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Fluxor;

namespace PlainTextEditor.ClassLib.Store.PlainTextEditorCase;

[FeatureState]
public partial record PlainTextEditorStates(ImmutableDictionary<PlainTextEditorKey, IPlainTextEditor> Map, 
    ImmutableArray<PlainTextEditorKey> Array)
{
    private PlainTextEditorStates() : this(new Dictionary<PlainTextEditorKey, IPlainTextEditor>().ToImmutableDictionary(),
        new PlainTextEditorKey[0].ToImmutableArray())
    {
        
    }
}
