using BlazorStudio.ClassLib.FileSystem.Classes;
using BlazorStudio.ClassLib.Store.SolutionExplorer;
using BlazorStudio.ClassLib.Store.TerminalCase;
using BlazorTextEditor.RazorLib;
using BlazorTextEditor.RazorLib.Store.ThemeCase;
using Fluxor;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib;

public partial class BlazorTextEditorRazorLibInitializer : ComponentBase
{
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;
    
    protected override void OnInitialized()
    {
        TextEditorService.SetTheme(ThemeFacts.BlazorTextEditorDark);
        
        foreach (var terminalSessionKey in TerminalSessionFacts.WELL_KNOWN_TERMINAL_SESSION_KEYS)
        {
            var terminalSession = new TerminalSession(
                null, 
                Dispatcher)
            {
                TerminalSessionKey = terminalSessionKey
            };
         
            Dispatcher.Dispatch(new TerminalSessionsReducer.RegisterTerminalSessionAction(
                terminalSession));
        }

        // This block is so I can work on the Solution Explorer UI
        // without clicking through the app to open a solution
        {
            var testSolutionExplorer = new AbsoluteFilePath(
                @"C:\Users\hunte\Repos\Demos\BlazorCrudApp\BlazorCrudApp.sln",
                false);

            if (System.IO.File.Exists(testSolutionExplorer.GetAbsoluteFilePathString()))
            {
                Dispatcher.Dispatch(new SolutionExplorerState.RequestSetSolutionExplorerStateAction(
                    testSolutionExplorer));
            }
        }
        
        base.OnInitialized();
    }
}