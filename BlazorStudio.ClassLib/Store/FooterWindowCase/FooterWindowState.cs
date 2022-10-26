using System.Collections.Immutable;
using Fluxor;

namespace BlazorStudio.ClassLib.Store.FooterWindowCase;

[FeatureState]
public record FooterWindowState(FooterWindowKind ActiveFooterWindowKind)
{
    public FooterWindowState() : this(FooterWindowKind.Terminal)
    {
    }

    public ImmutableArray<FooterWindowTabEntry> GetFooterWindowTabEntries()
    {
        return Enum
            .GetValues(typeof(FooterWindowKind))
            .Cast<FooterWindowKind>()
            .Select(x => new FooterWindowTabEntry(x, x.ToString()))
            .ToImmutableArray();
    }
}