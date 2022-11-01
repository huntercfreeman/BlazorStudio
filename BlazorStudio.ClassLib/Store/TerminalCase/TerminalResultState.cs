using System.Collections.Immutable;
using Fluxor;

namespace BlazorStudio.ClassLib.Store.TerminalCase;

[FeatureState]
public record TerminalResultState(
    ImmutableDictionary<TerminalCommandKey, TerminalCommand> TerminalCommandResultMap)
{

    public TerminalResultState() 
        : this(ImmutableDictionary<TerminalCommandKey, TerminalCommand>.Empty)
    {
    }
}