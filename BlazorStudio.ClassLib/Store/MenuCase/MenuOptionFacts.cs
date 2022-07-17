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

        public static readonly MenuOptionRecord OpenFile = new MenuOptionRecord(MenuOptionKey.NewMenuOptionKey(),
            "Open File",
            ImmutableList<MenuOptionRecord>.Empty);

        public static readonly MenuOptionRecord Open = new MenuOptionRecord(MenuOptionKey.NewMenuOptionKey(),
            "Open",
            new MenuOptionRecord[]
            {
                OpenFolder,
                OpenFile
            }.ToImmutableList());
    }
}