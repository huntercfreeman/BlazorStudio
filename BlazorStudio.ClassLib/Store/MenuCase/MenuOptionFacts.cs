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
                "File",
                ImmutableList<MenuOptionRecord>.Empty,
                null,
                WidgetType: widgetType,
                WidgetParameters: widgetParameters);
        
        public static MenuOptionRecord ConstructCreateNewDirectory(Type widgetType, Dictionary<string, object?>? widgetParameters) => 
            new(MenuOptionKey.NewMenuOptionKey(),
                "New Directory",
                ImmutableList<MenuOptionRecord>.Empty,
                null,
                WidgetType: widgetType,
                WidgetParameters: widgetParameters);
    }
    
    public static class CSharp
    {
        public static MenuOptionRecord ConstructCreateNewCSharpProject(Action onClickAction) => new(MenuOptionKey.NewMenuOptionKey(),
            "C# Project",
            ImmutableList<MenuOptionRecord>.Empty,
            onClickAction);
    }

    public static MenuOptionRecord NewMenu(Action onNewCSharpProject) => new(MenuOptionKey.NewMenuOptionKey(),
        "New",
        new MenuOptionRecord[]
        {
            CSharp.ConstructCreateNewCSharpProject(onNewCSharpProject),
            File.ConstructCreateNewFile(null, null)
        }.ToImmutableList(),
        null);
}