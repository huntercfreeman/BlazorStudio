using System.Collections.Immutable;

namespace BlazorStudio.ClassLib.Store.MenuCase;

public static class MenuOptionFacts
{
    public static class File
    {
        public static MenuOptionRecord ConstructOpenFolder(Action onClickAction)
        {
            return new(MenuOptionKey.NewMenuOptionKey(),
                "Open Folder",
                ImmutableList<MenuOptionRecord>.Empty,
                onClickAction,
                MenuOptionKind.Read);
        }

        public static MenuOptionRecord ConstructOpenFile(Action onClickAction)
        {
            return new(MenuOptionKey.NewMenuOptionKey(),
                "Open File",
                ImmutableList<MenuOptionRecord>.Empty,
                onClickAction,
                MenuOptionKind.Read);
        }

        public static MenuOptionRecord ConstructCreateNewEmptyFile(Type widgetType,
            Dictionary<string, object?>? widgetParameters)
        {
            return new(MenuOptionKey.NewMenuOptionKey(),
                "New Empty File",
                ImmutableList<MenuOptionRecord>.Empty,
                null,
                MenuOptionKind.Create,
                widgetType,
                widgetParameters);
        }

        public static MenuOptionRecord ConstructCreateNewTemplatedFile(Type widgetType,
            Dictionary<string, object?>? widgetParameters)
        {
            return new(MenuOptionKey.NewMenuOptionKey(),
                "New Templated File",
                ImmutableList<MenuOptionRecord>.Empty,
                null,
                MenuOptionKind.Create,
                widgetType,
                widgetParameters);
        }

        public static MenuOptionRecord ConstructCreateNewDirectory(Type widgetType,
            Dictionary<string, object?>? widgetParameters)
        {
            return new(MenuOptionKey.NewMenuOptionKey(),
                "New Directory",
                ImmutableList<MenuOptionRecord>.Empty,
                null,
                MenuOptionKind.Create,
                widgetType,
                widgetParameters);
        }

        public static MenuOptionRecord ConstructDeleteFile(Type widgetType,
            Dictionary<string, object?>? widgetParameters)
        {
            return new(MenuOptionKey.NewMenuOptionKey(),
                "Delete File",
                ImmutableList<MenuOptionRecord>.Empty,
                null,
                MenuOptionKind.Delete,
                widgetType,
                widgetParameters);
        }
    }

    public static class CSharp
    {
        public static MenuOptionRecord ConstructCreateNewCSharpProject(Action onClickAction)
        {
            return new(MenuOptionKey.NewMenuOptionKey(),
                "C# Project",
                ImmutableList<MenuOptionRecord>.Empty,
                onClickAction,
                MenuOptionKind.Create);
        }

        public static MenuOptionRecord RenderSyntaxRoot(Action onClickAction)
        {
            return new(MenuOptionKey.NewMenuOptionKey(),
                "Render SyntaxRoot",
                ImmutableList<MenuOptionRecord>.Empty,
                onClickAction,
                MenuOptionKind.Read);
        }

        public static MenuOptionRecord SetAsStartupProject(Action onClickAction)
        {
            return new(MenuOptionKey.NewMenuOptionKey(),
                "Set as Startup Project",
                ImmutableList<MenuOptionRecord>.Empty,
                onClickAction,
                MenuOptionKind.Update);
        }

        public static MenuOptionRecord AddProjectReference(Action onClickAction)
        {
            return new(MenuOptionKey.NewMenuOptionKey(),
                "Add Project Reference",
                ImmutableList<MenuOptionRecord>.Empty,
                onClickAction,
                MenuOptionKind.Update);
        }
    }

    public static class DotNet
    {
        public static MenuOptionRecord SetActiveSolution(Action onClickAction)
        {
            return new(MenuOptionKey.NewMenuOptionKey(),
                "Set Active Solution",
                ImmutableList<MenuOptionRecord>.Empty,
                onClickAction,
                MenuOptionKind.Update);
        }

        public static MenuOptionRecord ConstructCreateNewDotNetSolution(Action onClickAction)
        {
            return new(MenuOptionKey.NewMenuOptionKey(),
                "Solution",
                ImmutableList<MenuOptionRecord>.Empty,
                onClickAction,
                MenuOptionKind.Create);
        }
    }

    public static MenuOptionRecord NewMenu(Action onNewCSharpProject, Action onNewDotNetSolution)
    {
        return new(MenuOptionKey.NewMenuOptionKey(),
            "New",
            new MenuOptionRecord[]
            {
                CSharp.ConstructCreateNewCSharpProject(onNewCSharpProject),
                DotNet.ConstructCreateNewDotNetSolution(onNewDotNetSolution),
            }.ToImmutableList(),
            null,
            MenuOptionKind.Create);
    }
}