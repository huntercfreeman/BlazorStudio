using System.Collections.Immutable;
using BlazorStudio.ClassLib.Commands;
using BlazorStudio.ClassLib.Keyboard;
using BlazorStudio.ClassLib.Store.CommandCase.Focus;

namespace BlazorStudio.ClassLib.Contexts;

public static class ContextFacts
{
    public static readonly ContextRecord GlobalContext = new ContextRecord(
        ContextKey.NewContextKey(), 
        "Global",
        "global",
        new Keymap(new Dictionary<KeyDownEventRecord, CommandRecord>
        {
            {
                new KeyDownEventRecord("a", "KeyA", false, false, true),
                new CommandRecord(CommandKey.NewCommandKey(), "Test Global", "Test Global", () => Console.WriteLine("Test Global"))
            },
            {
                new KeyDownEventRecord("g", "KeyG", false, false, true),
                new CommandRecord(CommandKey.NewCommandKey(), "Focus -> Main Layout", "set-focus_main-layout", new FocusMainLayoutAction())
            },
            {
                new KeyDownEventRecord("f", "KeyF", false, false, true),
                new CommandRecord(CommandKey.NewCommandKey(), "Focus -> Folder Explorer", "set-focus_folder-explorer", new FocusFolderExplorerAction())
            },
            {
                new KeyDownEventRecord("s", "KeyS", false, false, true),
                new CommandRecord(CommandKey.NewCommandKey(), "Focus -> Solution Explorer", "set-focus_solution-explorer", new FocusSolutionExplorerAction())
            },
            {
                new KeyDownEventRecord("t", "KeyT", false, false, true),
                new CommandRecord(CommandKey.NewCommandKey(), "Focus -> Toolbar Display", "set-focus_toolbar-display", new FocusToolbarDisplayAction())
            },
            {
                new KeyDownEventRecord("e", "KeyE", false, false, true),
                new CommandRecord(CommandKey.NewCommandKey(), "Focus -> Editor Display", "set-focus_editor-display", new FocusEditorDisplayAction())
            },
            {
                new KeyDownEventRecord("r", "KeyR", false, false, true),
                new CommandRecord(CommandKey.NewCommandKey(), "Focus -> Terminal Display", "set-focus_terminal-display", new FocusTerminalDisplayAction())
            },
            {
                new KeyDownEventRecord("d", "KeyD", false, false, true),
                new CommandRecord(CommandKey.NewCommandKey(), "Focus -> Dialog Quick Select Display", "set-focus_dialog-quick-select-display", new FocusDialogQuickSelectDisplayAction())
            },
            {
                new KeyDownEventRecord("n", "KeyN", false, false, true),
                new CommandRecord(CommandKey.NewCommandKey(), "Focus -> Nuget Package Manager Display", "set-focus_nuget-package-manager-display", new FocusNugetPackageManagerDisplayAction())
            },
        }.ToImmutableDictionary()));
    
    public static readonly ContextRecord PlainTextEditorContext = new ContextRecord(
        ContextKey.NewContextKey(), 
        "PlainTextEditor",
        "plain-text-editor",
        new Keymap(new Dictionary<KeyDownEventRecord, CommandRecord>
        {
            {
                new KeyDownEventRecord("a", "KeyA", false, false, true),
                new CommandRecord(CommandKey.NewCommandKey(), "Test Plain Text Editor", "Test Plain Text Editor", () => Console.WriteLine("Test Plain Text Editor"))
            }
        }.ToImmutableDictionary()));
    
    public static readonly ContextRecord SolutionExplorerContext = new ContextRecord(
        ContextKey.NewContextKey(), 
        "SolutionExplorer",
        "solution-explorer",
        new Keymap(new Dictionary<KeyDownEventRecord, CommandRecord>
        {
            {
                new KeyDownEventRecord("a", "KeyA", false, false, true),
                new CommandRecord(CommandKey.NewCommandKey(), "Test Solution Explorer", "Test Solution Explorer", () => Console.WriteLine("Test Solution Explorer"))
            }
        }.ToImmutableDictionary()));
    
    public static readonly ContextRecord FolderExplorerContext = new ContextRecord(
        ContextKey.NewContextKey(), 
        "FolderExplorer",
        "folder-explorer",
        new Keymap(new Dictionary<KeyDownEventRecord, CommandRecord>
        {
            {
                new KeyDownEventRecord("a", "KeyA", false, false, true),
                new CommandRecord(CommandKey.NewCommandKey(), "Test Folder Explorer", "Test Folder Explorer", () => Console.WriteLine("Test Folder Explorer"))
            }
        }.ToImmutableDictionary()));
    
    public static readonly ContextRecord DialogDisplayContext = new ContextRecord(
        ContextKey.NewContextKey(), 
        "DialogDisplay",
        "dialog-display",
        new Keymap(new Dictionary<KeyDownEventRecord, CommandRecord>
        {
            {
                new KeyDownEventRecord("a", "KeyA", false, false, true),
                new CommandRecord(CommandKey.NewCommandKey(), "Test Dialog Display", "Test Dialog Display", () => Console.WriteLine("Test Dialog Display"))
            }
        }.ToImmutableDictionary()));
    
    public static readonly ContextRecord ToolbarDisplayContext = new ContextRecord(
        ContextKey.NewContextKey(), 
        "ToolbarDisplay",
        "toolbar-display",
        new Keymap(new Dictionary<KeyDownEventRecord, CommandRecord>
        {
            {
                new KeyDownEventRecord("a", "KeyA", false, false, true),
                new CommandRecord(CommandKey.NewCommandKey(), "Test Toolbar Display", "Test Toolbar Display", () => Console.WriteLine("Test Toolbar Display"))
            }
        }.ToImmutableDictionary()));
    
    public static readonly ContextRecord EditorDisplayContext = new ContextRecord(
        ContextKey.NewContextKey(), 
        "EditorDisplay",
        "editor-display",
        new Keymap(new Dictionary<KeyDownEventRecord, CommandRecord>
        {
            {
                new KeyDownEventRecord("a", "KeyA", false, false, true),
                new CommandRecord(CommandKey.NewCommandKey(), "Test Editor Display", "Test Editor Display", () => Console.WriteLine("Test Editor Display"))
            }
        }.ToImmutableDictionary()));
    
    public static readonly ContextRecord TerminalDisplayContext = new ContextRecord(
        ContextKey.NewContextKey(), 
        "TerminalDisplay",
        "terminal-display",
        new Keymap(new Dictionary<KeyDownEventRecord, CommandRecord>
        {
            {
                new KeyDownEventRecord("a", "KeyA", false, false, true),
                new CommandRecord(CommandKey.NewCommandKey(), "Test Terminal Display", "Test Terminal Display", () => Console.WriteLine("Test Terminal Display"))
            }
        }.ToImmutableDictionary()));
    
    public static readonly ContextRecord QuickSelectDisplayContext = new ContextRecord(
        ContextKey.NewContextKey(), 
        "Quick SelectDisplay",
        "quick-select-display",
        new Keymap(new Dictionary<KeyDownEventRecord, CommandRecord>
        {
            {
                new KeyDownEventRecord("a", "KeyA", false, false, true),
                new CommandRecord(CommandKey.NewCommandKey(), "Test Quick Select Display", "Test Quick Select Display", () => Console.WriteLine("Test Quick Select Display"))
            }
        }.ToImmutableDictionary()));
    
    public static readonly ContextRecord NugetPackageManagerDisplayContext = new ContextRecord(
        ContextKey.NewContextKey(), 
        "NugetPackageManagerDisplay",
        "nuget-package-manager-display",
        new Keymap(new Dictionary<KeyDownEventRecord, CommandRecord>
        {
            {
                new KeyDownEventRecord("a", "KeyA", false, false, true),
                new CommandRecord(CommandKey.NewCommandKey(), "Test NugetPackageManager Display", "Test NugetPackageManager Display", () => Console.WriteLine("Test NugetPackageManager Display"))
            }
        }.ToImmutableDictionary()));
}