using System.Collections.Immutable;

namespace BlazorStudio.ClassLib.Store.MenuCase;

public static class MenuOptionFacts
{
    public static readonly ImmutableArray<MenuOptionRecord> InitialMenuOptions = new MenuOptionRecord[]
    {
        File.OpenFolder
    }.ToImmutableArray();

    public static class File
    {
        public static readonly MenuOptionRecord OpenFolder = new MenuOptionRecord(MenuOptionKey.NewMenuOptionKey(),
            "Open Folder",
            ImmutableList<MenuOptionRecord>.Empty);
    }
}