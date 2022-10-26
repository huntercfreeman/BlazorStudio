using System.Collections.Immutable;
using Fluxor;

namespace BlazorStudio.ClassLib.Store.TreeViewCase;

[FeatureState]
public record TreeViewWrapStates(ImmutableDictionary<TreeViewWrapKey, ITreeViewWrap> Map)
{
    private TreeViewWrapStates()
        : this(new Dictionary<TreeViewWrapKey, ITreeViewWrap>().ToImmutableDictionary())
    {
    }
}