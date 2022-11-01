using System.Collections.Immutable;

namespace BlazorStudio.ClassLib.Menu;

public record MenuRecord(ImmutableArray<MenuOptionRecord> MenuOptions);