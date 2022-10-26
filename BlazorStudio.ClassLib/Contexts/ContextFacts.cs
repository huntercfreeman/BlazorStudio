using System.Collections.Immutable;
using BlazorStudio.ClassLib.Commands;
using BlazorStudio.ClassLib.Keyboard;
using BlazorStudio.ClassLib.Store.CommandCase.Focus;

namespace BlazorStudio.ClassLib.Contexts;

public static class ContextFacts
{
    public static readonly ContextRecord GlobalContext = new(
        ContextKey.NewContextKey(),
        "Global",
        "global",
        new Keymap(new Dictionary<KeyDownEventRecord, CommandRecord>
        {
            {
                new KeyDownEventRecord("g", "KeyG", false, false, true),
                new CommandRecord(CommandKey.NewCommandKey(), "Focus -> Main Layout", "set-focus_main-layout",
                    _ => new FocusMainLayoutAction())
            },
            {
                new KeyDownEventRecord("f", "KeyF", false, false, true),
                new CommandRecord(CommandKey.NewCommandKey(), "Focus -> Folder Explorer", "set-focus_folder-explorer",
                    _ => new FocusFolderExplorerAction())
            },
            {
                new KeyDownEventRecord("s", "KeyS", false, false, true),
                new CommandRecord(CommandKey.NewCommandKey(), "Focus -> Solution Explorer",
                    "set-focus_solution-explorer", _ => new FocusSolutionExplorerAction())
            },
            {
                new KeyDownEventRecord("t", "KeyT", false, false, true),
                new CommandRecord(CommandKey.NewCommandKey(), "Focus -> Toolbar Display", "set-focus_toolbar-display",
                    _ => new FocusToolbarDisplayAction())
            },
            {
                new KeyDownEventRecord("e", "KeyE", false, false, true),
                new CommandRecord(CommandKey.NewCommandKey(), "Focus -> Editor Display", "set-focus_editor-display",
                    _ => new FocusEditorDisplayAction())
            },
            {
                new KeyDownEventRecord("r", "KeyR", false, false, true),
                new CommandRecord(CommandKey.NewCommandKey(), "Focus -> Terminal Display", "set-focus_terminal-display",
                    _ => new FocusTerminalDisplayAction())
            },
            {
                new KeyDownEventRecord("d", "KeyD", false, false, true),
                new CommandRecord(CommandKey.NewCommandKey(), "Focus -> Dialog Quick Select Display",
                    "set-focus_dialog-quick-select-display", _ => new FocusDialogQuickSelectDisplayAction())
            },
            {
                new KeyDownEventRecord("n", "KeyN", false, false, true),
                new CommandRecord(CommandKey.NewCommandKey(), "Focus -> Nuget Package Manager Display",
                    "set-focus_nuget-package-manager-display", _ => new FocusNugetPackageManagerDisplayAction())
            },
        }.ToImmutableDictionary()));

    public static readonly ContextRecord SolutionExplorerContext = new(
        ContextKey.NewContextKey(),
        "SolutionExplorer",
        "solution-explorer",
        new Keymap(new Dictionary<KeyDownEventRecord, CommandRecord>
        {
            // Keymaps go here
        }.ToImmutableDictionary()));

    public static readonly ContextRecord FolderExplorerContext = new(
        ContextKey.NewContextKey(),
        "FolderExplorer",
        "folder-explorer",
        new Keymap(new Dictionary<KeyDownEventRecord, CommandRecord>
        {
            // Keymaps go here
        }.ToImmutableDictionary()));

    public static readonly ContextRecord DialogDisplayContext = new(
        ContextKey.NewContextKey(),
        "DialogDisplay",
        "dialog-display",
        new Keymap(new Dictionary<KeyDownEventRecord, CommandRecord>
        {
            // Keymaps go here
        }.ToImmutableDictionary()));

    public static readonly ContextRecord ToolbarDisplayContext = new(
        ContextKey.NewContextKey(),
        "ToolbarDisplay",
        "toolbar-display",
        new Keymap(new Dictionary<KeyDownEventRecord, CommandRecord>
        {
            // Keymaps go here
        }.ToImmutableDictionary()));

    public static readonly ContextRecord EditorDisplayContext = new(
        ContextKey.NewContextKey(),
        "EditorDisplay",
        "editor-display",
        new Keymap(new Dictionary<KeyDownEventRecord, CommandRecord>
        {
            // Keymaps go here
        }.ToImmutableDictionary()));

    public static readonly ContextRecord TerminalDisplayContext = new(
        ContextKey.NewContextKey(),
        "TerminalDisplay",
        "terminal-display",
        new Keymap(new Dictionary<KeyDownEventRecord, CommandRecord>
        {
            // Keymaps go here
        }.ToImmutableDictionary()));

    public static readonly ContextRecord QuickSelectDisplayContext = new(
        ContextKey.NewContextKey(),
        "Quick SelectDisplay",
        "quick-select-display",
        new Keymap(new Dictionary<KeyDownEventRecord, CommandRecord>
        {
            // Keymaps go here
        }.ToImmutableDictionary()));

    public static readonly ContextRecord NugetPackageManagerDisplayContext = new(
        ContextKey.NewContextKey(),
        "NugetPackageManagerDisplay",
        "nuget-package-manager-display",
        new Keymap(new Dictionary<KeyDownEventRecord, CommandRecord>
        {
            // Keymaps go here
        }.ToImmutableDictionary()));
}