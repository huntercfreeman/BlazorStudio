using System.Collections.Immutable;
using Fluxor;

namespace BlazorStudio.ClassLib.Store.QuickSelectCase;

[FeatureState]
public record QuickSelectState(bool IsDisplayed,
    ImmutableArray<IQuickSelectItem> QuickSelectItems,
    Func<IQuickSelectItem, Task> OnHoveredItemChangedFunc,
    Func<IQuickSelectItem, Task> OnItemSelectedFunc)
{
    public QuickSelectState() : this(false,
        ImmutableArray<IQuickSelectItem>.Empty,
        (item) => Task.CompletedTask,
        (item) => Task.CompletedTask)
    {
    }
}