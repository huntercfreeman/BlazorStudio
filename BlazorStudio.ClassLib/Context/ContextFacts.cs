using System.Collections.Immutable;

namespace BlazorStudio.ClassLib.Context;

public static class ContextFacts
{
    public static readonly ContextRecord GlobalContext = new(
        ContextKey.NewContextKey(),
        "Global",
        "global");
    
    public static readonly ContextRecord FolderExplorerContext = new(
        ContextKey.NewContextKey(),
        "Folder Explorer",
        "folder-explorer");
			
    public static readonly ContextRecord DialogDisplayContext = new(
        ContextKey.NewContextKey(),
        "Dialog Display",
        "dialog-display");
		
    public static readonly ContextRecord MainLayoutHeaderContext = new(
        ContextKey.NewContextKey(),
        "MainLayout Header",
        "main-layout-header");
		
    public static readonly ContextRecord MainLayoutFooterContext = new(
        ContextKey.NewContextKey(),
        "MainLayout Footer",
        "main-layout-footer");
		
    public static readonly ContextRecord EditorContext = new(
        ContextKey.NewContextKey(),
        "Editor",
        "editor");
		
    public static readonly ContextRecord TextEditorContext = new(
        ContextKey.NewContextKey(),
        "Text Editor",
        "text-editor");
    
    public static readonly ContextRecord TerminalContext = new(
        ContextKey.NewContextKey(),
        "Terminal",
        "terminal");
    
    public static readonly ImmutableArray<ContextRecord> ContextRecords = new [] 
    {
        MainLayoutHeaderContext,
        DialogDisplayContext,
        FolderExplorerContext,
        GlobalContext,
        EditorContext,
        TextEditorContext
    }.ToImmutableArray();
}