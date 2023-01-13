using BlazorStudio.ClassLib.Store.TerminalCase;

namespace BlazorStudio.ClassLib.Git;

public static class GitFacts
{
    public const string GIT_FOLDER_NAME = ".git";

    public const string UNTRACKED_FILES_TEXT_START = "Untracked files:";
    
    public static readonly TerminalCommandKey GitInitTerminalCommandKey = 
        TerminalCommandKey.NewTerminalCommandKey();
    
    public static readonly TerminalCommandKey GitStatusTerminalCommandKey = 
        TerminalCommandKey.NewTerminalCommandKey();
}