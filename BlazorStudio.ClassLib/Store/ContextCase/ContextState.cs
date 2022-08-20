using System.Collections.Immutable;
using BlazorStudio.ClassLib.Contexts;
using Fluxor;

namespace BlazorStudio.ClassLib.Store.ContextCase;

[FeatureState]
public record ContextState(ImmutableDictionary<ContextKey, ContextRecord> ContextRecords,
    ImmutableList<ContextRecord> ActiveContextRecords)
{
    public ContextState() : this(ImmutableDictionary<ContextKey, ContextRecord>.Empty,
        ImmutableList<ContextRecord>.Empty)
    {
        ContextRecords = new Dictionary<ContextKey, ContextRecord>
        {
            {
                ContextFacts.GlobalContext.ContextKey,
                ContextFacts.GlobalContext
            },
            {
                ContextFacts.SolutionExplorerContext.ContextKey,
                ContextFacts.SolutionExplorerContext
            },
            {
                ContextFacts.FolderExplorerContext.ContextKey,
                ContextFacts.FolderExplorerContext
            },
            {
                ContextFacts.PlainTextEditorContext.ContextKey,
                ContextFacts.PlainTextEditorContext
            },
            {
                ContextFacts.DialogDisplayContext.ContextKey,
                ContextFacts.DialogDisplayContext
            },
            {
                ContextFacts.ToolbarDisplayContext.ContextKey,
                ContextFacts.ToolbarDisplayContext
            },
        }.ToImmutableDictionary();

        ActiveContextRecords = new List<ContextRecord>
        {
            ContextFacts.GlobalContext
        }.ToImmutableList();
    }
}