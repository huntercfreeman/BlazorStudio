using System.Collections.Immutable;

namespace BlazorStudio.ClassLib.Store.MenuCase;

public static class MenuOptionFacts
{
    public static class File
    {
        public static MenuOptionRecord ConstructOpenFolder(Action onClickAction) => new(MenuOptionKey.NewMenuOptionKey(),
            "Open Folder",
            ImmutableList<MenuOptionRecord>.Empty,
            onClickAction,
            MenuOptionKind.Read);

        public static MenuOptionRecord ConstructOpenFile(Action onClickAction) => new(MenuOptionKey.NewMenuOptionKey(),
            "Open File",
            ImmutableList<MenuOptionRecord>.Empty,
            onClickAction,
            MenuOptionKind.Read);
        
        public static MenuOptionRecord ConstructCreateNewEmptyFile(Type widgetType, Dictionary<string, object?>? widgetParameters) => 
            new(MenuOptionKey.NewMenuOptionKey(),
                "New Empty File",
                ImmutableList<MenuOptionRecord>.Empty,
                null,
                MenuOptionKind.Create,
                WidgetType: widgetType,
                WidgetParameters: widgetParameters);
        
        public static MenuOptionRecord ConstructCreateNewTemplatedFile(Type widgetType, Dictionary<string, object?>? widgetParameters) => 
            new(MenuOptionKey.NewMenuOptionKey(),
                "New Templated File",
                ImmutableList<MenuOptionRecord>.Empty,
                null,
                MenuOptionKind.Create,
                WidgetType: widgetType,
                WidgetParameters: widgetParameters);
        
        public static MenuOptionRecord ConstructCreateNewDirectory(Type widgetType, Dictionary<string, object?>? widgetParameters) => 
            new(MenuOptionKey.NewMenuOptionKey(),
                "New Directory",
                ImmutableList<MenuOptionRecord>.Empty,
                null,
                MenuOptionKind.Create,
                WidgetType: widgetType,
                WidgetParameters: widgetParameters);
        
        public static MenuOptionRecord ConstructDeleteFile(Type widgetType, Dictionary<string, object?>? widgetParameters) => 
            new(MenuOptionKey.NewMenuOptionKey(),
                "Delete File",
                ImmutableList<MenuOptionRecord>.Empty,
                null,
                MenuOptionKind.Delete,
                WidgetType: widgetType,
                WidgetParameters: widgetParameters);
    }
    
    public static class CSharp
    {
        public static MenuOptionRecord ConstructCreateNewCSharpProject(Action onClickAction) => new(MenuOptionKey.NewMenuOptionKey(),
            "C# Project",
            ImmutableList<MenuOptionRecord>.Empty,
            onClickAction,
            MenuOptionKind.Create);
        
        public static MenuOptionRecord RenderSyntaxRoot(Action onClickAction) => new(MenuOptionKey.NewMenuOptionKey(),
            "Render SyntaxRoot",
            ImmutableList<MenuOptionRecord>.Empty,
            onClickAction,
            MenuOptionKind.Read);

        public static MenuOptionRecord SetAsStartupProject(Action onClickAction) => new(MenuOptionKey.NewMenuOptionKey(),
            "Set as Startup Project",
            ImmutableList<MenuOptionRecord>.Empty,
            onClickAction,
            MenuOptionKind.Update);
        
        public static MenuOptionRecord AddProjectReference(Action onClickAction) => new(MenuOptionKey.NewMenuOptionKey(),
            "Add Project Reference",
            ImmutableList<MenuOptionRecord>.Empty,
            onClickAction,
            MenuOptionKind.Update);
    }
    
    public static class DotNet
    {
        public static MenuOptionRecord SetActiveSolution(Action onClickAction) => new(MenuOptionKey.NewMenuOptionKey(),
            "Set Active Solution",
            ImmutableList<MenuOptionRecord>.Empty,
            onClickAction,
            MenuOptionKind.Update);

        public static MenuOptionRecord ConstructCreateNewDotNetSolution(Action onClickAction) => new(MenuOptionKey.NewMenuOptionKey(),
            "Solution",
            ImmutableList<MenuOptionRecord>.Empty,
            onClickAction,
            MenuOptionKind.Create);
    }

    public static MenuOptionRecord NewMenu(Action onNewCSharpProject, Action onNewDotNetSolution) => new(MenuOptionKey.NewMenuOptionKey(),
        "New",
        new MenuOptionRecord[]
        {
            CSharp.ConstructCreateNewCSharpProject(onNewCSharpProject),
            DotNet.ConstructCreateNewDotNetSolution(onNewDotNetSolution)
        }.ToImmutableList(),
        null,
        MenuOptionKind.Create);
}