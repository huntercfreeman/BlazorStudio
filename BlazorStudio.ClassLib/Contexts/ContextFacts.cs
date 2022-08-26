using System.Collections.Immutable;
using BlazorStudio.ClassLib.Commands;
using BlazorStudio.ClassLib.Keyboard;
using BlazorStudio.ClassLib.Store.CommandCase.Focus;
using BlazorStudio.ClassLib.Store.ContextCase;
using BlazorStudio.ClassLib.Store.KeyDownEventCase;
using BlazorStudio.ClassLib.Store.PlainTextEditorCase;

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
                new KeyDownEventRecord("g", "KeyG", false, false, true),
                new CommandRecord(CommandKey.NewCommandKey(), "Focus -> Main Layout", "set-focus_main-layout", (_) =>  new FocusMainLayoutAction())
            },
            {
                new KeyDownEventRecord("f", "KeyF", false, false, true),
                new CommandRecord(CommandKey.NewCommandKey(), "Focus -> Folder Explorer", "set-focus_folder-explorer", (_) =>  new FocusFolderExplorerAction())
            },
            {
                new KeyDownEventRecord("s", "KeyS", false, false, true),
                new CommandRecord(CommandKey.NewCommandKey(), "Focus -> Solution Explorer", "set-focus_solution-explorer", (_) =>  new FocusSolutionExplorerAction())
            },
            {
                new KeyDownEventRecord("t", "KeyT", false, false, true),
                new CommandRecord(CommandKey.NewCommandKey(), "Focus -> Toolbar Display", "set-focus_toolbar-display", (_) =>  new FocusToolbarDisplayAction())
            },
            {
                new KeyDownEventRecord("e", "KeyE", false, false, true),
                new CommandRecord(CommandKey.NewCommandKey(), "Focus -> Editor Display", "set-focus_editor-display", (_) =>  new FocusEditorDisplayAction())
            },
            {
                new KeyDownEventRecord("r", "KeyR", false, false, true),
                new CommandRecord(CommandKey.NewCommandKey(), "Focus -> Terminal Display", "set-focus_terminal-display", (_) =>  new FocusTerminalDisplayAction())
            },
            {
                new KeyDownEventRecord("d", "KeyD", false, false, true),
                new CommandRecord(CommandKey.NewCommandKey(), "Focus -> Dialog Quick Select Display", "set-focus_dialog-quick-select-display", (_) =>  new FocusDialogQuickSelectDisplayAction())
            },
            {
                new KeyDownEventRecord("n", "KeyN", false, false, true),
                new CommandRecord(CommandKey.NewCommandKey(), "Focus -> Nuget Package Manager Display", "set-focus_nuget-package-manager-display", (_) => new FocusNugetPackageManagerDisplayAction())
            },
        }.ToImmutableDictionary()));
    
    public static readonly ContextRecord PlainTextEditorContext = new ContextRecord(
        ContextKey.NewContextKey(), 
        "PlainTextEditor",
        "plain-text-editor",
        new Keymap(new Dictionary<KeyDownEventRecord, CommandRecord>
        {
            {
                new KeyDownEventRecord("s", "KeyS", true, false, false),
                new CommandRecord(CommandKey.NewCommandKey(), 
                    "Save", 
                    "save-changes-of-active-plain-text-editor-file", 
                    (keymapEventAction) =>
                    {
                        if (keymapEventAction.Parameters is PlainTextEditorKey plainTextEditorKey)
                        {
                            return new PlainTextEditorOnSaveRequestedAction(
                                plainTextEditorKey,
                                keymapEventAction.CancellationToken);
                        }

                        return new VoidAction();
                    })
            }
        }.ToImmutableDictionary()));
    
    public static readonly ContextRecord SolutionExplorerContext = new ContextRecord(
        ContextKey.NewContextKey(), 
        "SolutionExplorer",
        "solution-explorer",
        new Keymap(new Dictionary<KeyDownEventRecord, CommandRecord>
        {
            // Keymaps go here
        }.ToImmutableDictionary()));
    
    public static readonly ContextRecord FolderExplorerContext = new ContextRecord(
        ContextKey.NewContextKey(), 
        "FolderExplorer",
        "folder-explorer",
        new Keymap(new Dictionary<KeyDownEventRecord, CommandRecord>
        {
            // Keymaps go here
        }.ToImmutableDictionary()));
    
    public static readonly ContextRecord DialogDisplayContext = new ContextRecord(
        ContextKey.NewContextKey(), 
        "DialogDisplay",
        "dialog-display",
        new Keymap(new Dictionary<KeyDownEventRecord, CommandRecord>
        {
            // Keymaps go here
        }.ToImmutableDictionary()));
    
    public static readonly ContextRecord ToolbarDisplayContext = new ContextRecord(
        ContextKey.NewContextKey(), 
        "ToolbarDisplay",
        "toolbar-display",
        new Keymap(new Dictionary<KeyDownEventRecord, CommandRecord>
        {
            // Keymaps go here
        }.ToImmutableDictionary()));
    
    public static readonly ContextRecord EditorDisplayContext = new ContextRecord(
        ContextKey.NewContextKey(), 
        "EditorDisplay",
        "editor-display",
        new Keymap(new Dictionary<KeyDownEventRecord, CommandRecord>
        {
            // Keymaps go here
        }.ToImmutableDictionary()));
    
    public static readonly ContextRecord TerminalDisplayContext = new ContextRecord(
        ContextKey.NewContextKey(), 
        "TerminalDisplay",
        "terminal-display",
        new Keymap(new Dictionary<KeyDownEventRecord, CommandRecord>
        {
            // Keymaps go here
        }.ToImmutableDictionary()));
    
    public static readonly ContextRecord QuickSelectDisplayContext = new ContextRecord(
        ContextKey.NewContextKey(), 
        "Quick SelectDisplay",
        "quick-select-display",
        new Keymap(new Dictionary<KeyDownEventRecord, CommandRecord>
        {
            // Keymaps go here
        }.ToImmutableDictionary()));
    
    public static readonly ContextRecord NugetPackageManagerDisplayContext = new ContextRecord(
        ContextKey.NewContextKey(), 
        "NugetPackageManagerDisplay",
        "nuget-package-manager-display",
        new Keymap(new Dictionary<KeyDownEventRecord, CommandRecord>
        {
            // Keymaps go here
        }.ToImmutableDictionary()));
}