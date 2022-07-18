using System.Collections.Immutable;

namespace BlazorStudio.ClassLib.Store.MenuCase;

public record MenuOptionRecord(MenuOptionKey MenuOptionKey,
    string DisplayName,
    ImmutableList<MenuOptionRecord> Children,
    Action? OnClickAction,
    Type? Widget = null);