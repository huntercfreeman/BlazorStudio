using System.Collections.Immutable;
using BlazorStudio.ClassLib.Panel;
using Fluxor;

namespace BlazorStudio.ClassLib.Store.PanelCase;

[FeatureState]
public partial record PanelsCollection
{
    public PanelsCollection()
    {
        PanelRecordsList = ImmutableArray<PanelRecord>.Empty;
    }

    public ImmutableArray<PanelRecord> PanelRecordsList { get; init; }
}