using System.Collections.Immutable;

namespace BlazorStudio.ClassLib.Store.MenuCase;

public record MenuOptionRecord(MenuOptionKey MenuOptionKey,
    ImmutableList<MenuOptionRecord> Children);