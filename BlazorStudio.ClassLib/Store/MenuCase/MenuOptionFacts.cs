using System.Collections.Immutable;

namespace BlazorStudio.ClassLib.Store.MenuCase;

public static class MenuOptionFacts
{
    public static class File
    {
        public static MenuOptionRecord ConstructOpenFolder(Action onClickAction) => new(MenuOptionKey.NewMenuOptionKey(),
            "Open Folder",
            ImmutableList<MenuOptionRecord>.Empty,
            onClickAction);

        public static MenuOptionRecord ConstructOpenFile(Action onClickAction) => new(MenuOptionKey.NewMenuOptionKey(),
            "Open File",
            ImmutableList<MenuOptionRecord>.Empty,
            onClickAction);
        
        public static MenuOptionRecord ConstructCreateNewFile(Type widgetType, Dictionary<string, object?>? widgetParameters) => 
            new(MenuOptionKey.NewMenuOptionKey(),
                "New File",
                ImmutableList<MenuOptionRecord>.Empty,
                null,
                WidgetType: widgetType,
                WidgetParameters: widgetParameters);
    }
}