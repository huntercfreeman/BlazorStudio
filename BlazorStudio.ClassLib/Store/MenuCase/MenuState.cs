using System.Collections.Immutable;
using Fluxor;

namespace BlazorStudio.ClassLib.Store.MenuCase;

[FeatureState]
public record MenuState(ImmutableList<MenuOptionRecord> MenuRecords)
{
    public MenuState() : this(ImmutableList<MenuOptionRecord>.Empty)
    {
    }
}