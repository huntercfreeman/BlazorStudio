namespace BlazorStudio.ClassLib.Contexts;

public static class ContextFacts
{
    public static readonly ContextRecord GlobalContext = new ContextRecord(
        ContextKey.NewContextKey(), 
        "Global",
        "global");
    
    public static readonly ContextRecord PlainTextEditorContext = new ContextRecord(
        ContextKey.NewContextKey(), 
        "PlainTextEditor",
        "plain-text-editor");
    
    public static readonly ContextRecord SolutionExplorerContext = new ContextRecord(
        ContextKey.NewContextKey(), 
        "SolutionExplorer",
        "solution-explorer");
    
    public static readonly ContextRecord FolderExplorerContext = new ContextRecord(
        ContextKey.NewContextKey(), 
        "FolderExplorer",
        "folder-explorer");
}