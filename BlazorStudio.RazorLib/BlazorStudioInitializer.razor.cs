using BlazorALaCarte.Shared.Facts;
using BlazorALaCarte.Shared.Icons.Codicon;
using BlazorStudio.ClassLib.FileSystem.Classes;
using BlazorStudio.ClassLib.Panel;
using BlazorStudio.ClassLib.Store.PanelCase;
using BlazorStudio.ClassLib.Store.SolutionExplorer;
using BlazorStudio.ClassLib.Store.TerminalCase;
using BlazorStudio.RazorLib.FolderExplorer;
using BlazorStudio.RazorLib.SolutionExplorer;
using BlazorTextEditor.RazorLib;
using Fluxor;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib;

public partial class BlazorStudioInitializer : ComponentBase
{
    [Inject]
    private IState<PanelsCollection> PanelsCollectionWrap { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;
    
    protected override void OnInitialized()
    {
        TextEditorService.SetTheme(ThemeFacts.VisualStudioDarkThemeClone);
        
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

        InitializePanelTabs();
        
        base.OnInitialized();
    }

    private void InitializePanelTabs()
    {
        InitializeLeftPanelTabs();
        InitializeRightPanelTabs();
        InitializeBottomPanelTabs();
    }
    
    private void InitializeLeftPanelTabs()
    {
        var leftPanel = PanelFacts.GetLeftPanelRecord(PanelsCollectionWrap.Value);

        var solutionExplorerPanelTab = new PanelTab(
            PanelTabKey.NewPanelTabKey(),
            leftPanel.ElementDimensions,
            typeof(SolutionExplorerDisplay),
            typeof(IconFolder),
            "Solution Explorer");
        
        Dispatcher.Dispatch(new PanelsCollection.RegisterPanelTabAction(
            leftPanel.PanelRecordKey,
            solutionExplorerPanelTab));
        
        var folderExplorerPanelTab = new PanelTab(
            PanelTabKey.NewPanelTabKey(),
            leftPanel.ElementDimensions,
            typeof(FolderExplorerDisplay),
            typeof(IconFolder),
            "Folder Explorer");
        
        Dispatcher.Dispatch(new PanelsCollection.RegisterPanelTabAction(
            leftPanel.PanelRecordKey,
            folderExplorerPanelTab));
        
        var gitChangesPanelTab = new PanelTab(
            PanelTabKey.NewPanelTabKey(),
            leftPanel.ElementDimensions,
            typeof(FolderExplorerDisplay),
            typeof(IconFolder),
            "Git Changes");
        
        Dispatcher.Dispatch(new PanelsCollection.RegisterPanelTabAction(
            leftPanel.PanelRecordKey,
            gitChangesPanelTab));
    }
    
    private void InitializeRightPanelTabs()
    {
        var rightPanel = PanelFacts.GetRightPanelRecord(PanelsCollectionWrap.Value);

        var notificationsPanelTab = new PanelTab(
            PanelTabKey.NewPanelTabKey(),
            rightPanel.ElementDimensions,
            typeof(SolutionExplorerDisplay),
            typeof(IconFolder),
            "Notifications");
        
        Dispatcher.Dispatch(new PanelsCollection.RegisterPanelTabAction(
            rightPanel.PanelRecordKey,
            notificationsPanelTab));
    }
    
    private void InitializeBottomPanelTabs()
    {
        var bottomPanel = PanelFacts.GetBottomPanelRecord(PanelsCollectionWrap.Value);

        var gitPanelTab = new PanelTab(
            PanelTabKey.NewPanelTabKey(),
            bottomPanel.ElementDimensions,
            typeof(SolutionExplorerDisplay),
            typeof(IconFolder),
            "Git");
        
        Dispatcher.Dispatch(new PanelsCollection.RegisterPanelTabAction(
            bottomPanel.PanelRecordKey,
            gitPanelTab));
        
        var buildPanelTab = new PanelTab(
            PanelTabKey.NewPanelTabKey(),
            bottomPanel.ElementDimensions,
            typeof(SolutionExplorerDisplay),
            typeof(IconFolder),
            "Build");
        
        Dispatcher.Dispatch(new PanelsCollection.RegisterPanelTabAction(
            bottomPanel.PanelRecordKey,
            buildPanelTab));
        
        var terminalPanelTab = new PanelTab(
            PanelTabKey.NewPanelTabKey(),
            bottomPanel.ElementDimensions,
            typeof(SolutionExplorerDisplay),
            typeof(IconFolder),
            "Terminal");
        
        Dispatcher.Dispatch(new PanelsCollection.RegisterPanelTabAction(
            bottomPanel.PanelRecordKey,
            terminalPanelTab));
        
        var nuGetPanelTab = new PanelTab(
            PanelTabKey.NewPanelTabKey(),
            bottomPanel.ElementDimensions,
            typeof(SolutionExplorerDisplay),
            typeof(IconFolder),
            "NuGet");
        
        Dispatcher.Dispatch(new PanelsCollection.RegisterPanelTabAction(
            bottomPanel.PanelRecordKey,
            nuGetPanelTab));
        
        var unitTestsPanelTab = new PanelTab(
            PanelTabKey.NewPanelTabKey(),
            bottomPanel.ElementDimensions,
            typeof(SolutionExplorerDisplay),
            typeof(IconFolder),
            "Unit Tests");
        
        Dispatcher.Dispatch(new PanelsCollection.RegisterPanelTabAction(
            bottomPanel.PanelRecordKey,
            unitTestsPanelTab));
        
        var problemsPanelTab = new PanelTab(
            PanelTabKey.NewPanelTabKey(),
            bottomPanel.ElementDimensions,
            typeof(SolutionExplorerDisplay),
            typeof(IconFolder),
            "Problems");
        
        Dispatcher.Dispatch(new PanelsCollection.RegisterPanelTabAction(
            bottomPanel.PanelRecordKey,
            problemsPanelTab));
    }
}