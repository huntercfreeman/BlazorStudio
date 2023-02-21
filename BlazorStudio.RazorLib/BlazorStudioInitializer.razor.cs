using BlazorALaCarte.Shared.Icons.Codicon;
using BlazorStudio.ClassLib.FileSystem.Classes;
using BlazorStudio.ClassLib.FileSystem.Classes.FilePath;
using BlazorStudio.ClassLib.FileSystem.Interfaces;
using BlazorStudio.ClassLib.Panel;
using BlazorStudio.ClassLib.Store.PanelCase;
using BlazorStudio.ClassLib.Store.SolutionExplorer;
using BlazorStudio.ClassLib.Store.TerminalCase;
using BlazorStudio.RazorLib.FolderExplorer;
using BlazorStudio.RazorLib.Git;
using BlazorStudio.RazorLib.Notification;
using BlazorStudio.RazorLib.NuGet;
using BlazorStudio.RazorLib.SolutionExplorer;
using BlazorStudio.RazorLib.Terminal;
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
    [Inject]
    private IFileSystemProvider FileSystemProvider { get; set; } = null!;
    [Inject]
    private IEnvironmentProvider EnvironmentProvider { get; set; } = null!;
    
    protected override void OnInitialized()
    {
        foreach (var terminalSessionKey in TerminalSessionFacts.WELL_KNOWN_TERMINAL_SESSION_KEYS)
        {
            var terminalSession = new TerminalSession(
                null, 
                Dispatcher,
                FileSystemProvider)
            {
                TerminalSessionKey = terminalSessionKey
            };
         
            Dispatcher.Dispatch(new TerminalSessionsReducer.RegisterTerminalSessionAction(
                terminalSession));
        }

        InitializePanelTabs();
        
        base.OnInitialized();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        // This block is so I can work on the Solution Explorer UI
        // without clicking through the app to open a solution
        // {
        //     var testSolutionExplorer = new AbsoluteFilePath(
        //         @"C:\Users\hunte\Repos\Demos\BlazorCrudApp\BlazorCrudApp.sln",
        //         false,
        //         EnvironmentProvider);
        //
        //     if (await FileSystemProvider.File.ExistsAsync(testSolutionExplorer.GetAbsoluteFilePathString()))
        //     {
        //         Dispatcher.Dispatch(new SolutionExplorerState.RequestSetSolutionExplorerStateAction(
        //             testSolutionExplorer,
        //             EnvironmentProvider));
        //     }
        // }
        
        await base.OnAfterRenderAsync(firstRender);
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
            new(),
            typeof(SolutionExplorerDisplay),
            typeof(IconFolder),
            "Solution Explorer");
        
        Dispatcher.Dispatch(new PanelsCollection.RegisterPanelTabAction(
            leftPanel.PanelRecordKey,
            solutionExplorerPanelTab));
        
        Dispatcher.Dispatch(new PanelsCollection.SetActivePanelTabAction(
            leftPanel.PanelRecordKey,
            solutionExplorerPanelTab.PanelTabKey));
        
        var folderExplorerPanelTab = new PanelTab(
            PanelTabKey.NewPanelTabKey(),
            leftPanel.ElementDimensions,
            new(),
            typeof(FolderExplorerDisplay),
            typeof(IconFolder),
            "Folder Explorer");
        
        Dispatcher.Dispatch(new PanelsCollection.RegisterPanelTabAction(
            leftPanel.PanelRecordKey,
            folderExplorerPanelTab));
        
        var gitChangesPanelTab = new PanelTab(
            PanelTabKey.NewPanelTabKey(),
            leftPanel.ElementDimensions,
            new(),
            typeof(GitChangesDisplay),
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
            new(),
            typeof(NotificationHistoryDisplay),
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
            new(),
            typeof(TerminalDisplay),
            typeof(IconFolder),
            "Git");
        
        Dispatcher.Dispatch(new PanelsCollection.RegisterPanelTabAction(
            bottomPanel.PanelRecordKey,
            gitPanelTab));
        
        var buildPanelTab = new PanelTab(
            PanelTabKey.NewPanelTabKey(),
            bottomPanel.ElementDimensions,
            new(),
            typeof(TerminalDisplay),
            typeof(IconFolder),
            "Build");
        
        Dispatcher.Dispatch(new PanelsCollection.RegisterPanelTabAction(
            bottomPanel.PanelRecordKey,
            buildPanelTab));
        
        var terminalPanelTab = new PanelTab(
            PanelTabKey.NewPanelTabKey(),
            bottomPanel.ElementDimensions,
            new(),
            typeof(TerminalDisplay),
            typeof(IconFolder),
            "Terminal");
        
        Dispatcher.Dispatch(new PanelsCollection.RegisterPanelTabAction(
            bottomPanel.PanelRecordKey,
            terminalPanelTab));
        
        var nuGetPanelTab = new PanelTab(
            PanelTabKey.NewPanelTabKey(),
            bottomPanel.ElementDimensions,
            new(),
            typeof(NuGetPackageManager),
            typeof(IconFolder),
            "NuGet");
        
        Dispatcher.Dispatch(new PanelsCollection.RegisterPanelTabAction(
            bottomPanel.PanelRecordKey,
            nuGetPanelTab));
        
        Dispatcher.Dispatch(new PanelsCollection.SetActivePanelTabAction(
            bottomPanel.PanelRecordKey,
            nuGetPanelTab.PanelTabKey));
        
        var unitTestsPanelTab = new PanelTab(
            PanelTabKey.NewPanelTabKey(),
            bottomPanel.ElementDimensions,
            new(),
            typeof(TerminalDisplay),
            typeof(IconFolder),
            "Unit Tests");
        
        Dispatcher.Dispatch(new PanelsCollection.RegisterPanelTabAction(
            bottomPanel.PanelRecordKey,
            unitTestsPanelTab));
        
        var problemsPanelTab = new PanelTab(
            PanelTabKey.NewPanelTabKey(),
            bottomPanel.ElementDimensions,
            new(),
            typeof(TerminalDisplay),
            typeof(IconFolder),
            "Problems");
        
        Dispatcher.Dispatch(new PanelsCollection.RegisterPanelTabAction(
            bottomPanel.PanelRecordKey,
            problemsPanelTab));
    }
}