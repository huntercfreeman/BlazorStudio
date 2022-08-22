using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using Fluxor;

namespace BlazorStudio.ClassLib.Store.QuickSelectCase;

[FeatureState]
public record QuickSelectState(ImmutableArray<IQuickSelectItem> QuickSelectItems,
    Func<IQuickSelectItem, Task> OnHoveredItemChangedFunc,
    Func<IQuickSelectItem, Task> OnItemSelectedFunc)
{
    public QuickSelectState() : this(ImmutableArray<IQuickSelectItem>.Empty,
        (item) => Task.CompletedTask,
        (item) => Task.CompletedTask)
    {
        
    }
}

public class QuickSelectStateReducer
{
    [ReducerMethod]
    private QuickSelectState ReduceSetQuickSelectStateAction(QuickSelectState previousQuickSelectState,
        SetQuickSelectStateAction setQuickSelectStateAction)
    {
        return setQuickSelectStateAction.QuickSelectState;
    }
}

public record SetQuickSelectStateAction(QuickSelectState QuickSelectState);

public record QuickSelectItem<T>
    : IQuickSelectItem
{
    public QuickSelectItem(string displayName, [DisallowNull] T item)
    {
        DisplayName = displayName ?? throw new ArgumentNullException(nameof(displayName));
        ItemNoType = item ?? throw new ArgumentNullException(nameof(item));
        ItemWithType = item;
    }
    
    public string DisplayName { get; init; }
    public object ItemNoType { get; init; }
    public Type ItemType => typeof(T);
    public T ItemWithType { get; init; }
}

public interface IQuickSelectItem
{
    public string DisplayName { get; }
    public object ItemNoType { get; }
    public Type ItemType { get; }
}