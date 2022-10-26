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
                ContextFacts.DialogDisplayContext.ContextKey,
                ContextFacts.DialogDisplayContext
            },
            {
                ContextFacts.ToolbarDisplayContext.ContextKey,
                ContextFacts.ToolbarDisplayContext
            },
            {
                ContextFacts.EditorDisplayContext.ContextKey,
                ContextFacts.EditorDisplayContext
            },
            {
                ContextFacts.TerminalDisplayContext.ContextKey,
                ContextFacts.TerminalDisplayContext
            },
            {
                ContextFacts.NugetPackageManagerDisplayContext.ContextKey,
                ContextFacts.NugetPackageManagerDisplayContext
            },
        }.ToImmutableDictionary();

        ActiveContextRecords = new List<ContextRecord>
        {
            ContextFacts.GlobalContext,
        }.ToImmutableList();
    }
}