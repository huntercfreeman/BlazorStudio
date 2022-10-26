using System.Diagnostics.CodeAnalysis;

namespace BlazorStudio.ClassLib.Store.QuickSelectCase;

public record QuickSelectItem<T>
    : IQuickSelectItem
{
    public QuickSelectItem(string displayName, [DisallowNull] T item)
    {
        DisplayName = displayName ?? throw new ArgumentNullException(nameof(displayName));
        ItemNoType = item ?? throw new ArgumentNullException(nameof(item));
        ItemWithType = item;
    }

    public T ItemWithType { get; init; }

    public string DisplayName { get; init; }
    public object ItemNoType { get; init; }
    public Type ItemType => typeof(T);
}