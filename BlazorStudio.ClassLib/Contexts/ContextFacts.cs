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
                new KeyDownEventRecord("f", "KeyF", false, false, true),
                new CommandRecord(CommandKey.NewCommandKey(), "Focus -> Folder Explorer", "set-focus_folder-explorer", new FocusFolderExplorerAction())
            },
            {
                new KeyDownEventRecord("g", "KeyG", false, false, true),
                new CommandRecord(CommandKey.NewCommandKey(), "Focus -> Main Layout", "set-focus_main-layout", new FocusMainLayoutAction())
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
}