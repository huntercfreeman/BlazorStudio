using System.Collections.Immutable;
using BlazorALaCarte.DialogNotification;
using BlazorALaCarte.DialogNotification.Dialog;
using BlazorALaCarte.DialogNotification.Store.DialogCase;
using BlazorALaCarte.Shared.Dropdown;
using BlazorALaCarte.Shared.Menu;
using BlazorALaCarte.Shared.Store.DropdownCase;
using BlazorStudio.ClassLib.CommonComponents;
using BlazorStudio.ClassLib.FileSystem.Interfaces;
using BlazorStudio.ClassLib.Store.AccountCase;
using BlazorStudio.ClassLib.Store.EditorCase;
using BlazorStudio.ClassLib.Store.FolderExplorerCase;
using BlazorStudio.ClassLib.Store.InputFileCase;
using BlazorStudio.ClassLib.Store.SolutionExplorer;
using BlazorStudio.RazorLib.Account;
using BlazorStudio.RazorLib.Button;
using BlazorStudio.RazorLib.DotNetSolutionForm;
using BlazorStudio.RazorLib.InputFile;
using BlazorTextEditor.RazorLib;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.Shared;

public partial class BlazorTextEditorHeader : FluxorComponent
{
    [Inject]
    private IState<AccountState> AccountStateWrap { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;
    [Inject]
    private ICommonComponentRenderers CommonComponentRenderers { get; set; } = null!;
    [Inject]
    private IFileSystemProvider FileSystemProvider { get; set; } = null!;
    [Inject]
    private IEnvironmentProvider EnvironmentProvider { get; set; } = null!;

    [Parameter, EditorRequired]
    public Type LoginDisplayComponentType { get; set; } = null!;
    
    private DropdownKey _dropdownKeyFile = DropdownKey.NewDropdownKey();
    private MenuRecord _menuFile = new(ImmutableArray<MenuOptionRecord>.Empty);
    private ButtonDisplay? _buttonDisplayComponentFile;
    
    private DropdownKey _dropdownKeyAccount = DropdownKey.NewDropdownKey();
    private MenuRecord _menuAccount = new(ImmutableArray<MenuOptionRecord>.Empty);
    private ButtonDisplay? _buttonDisplayComponentAccount;

    protected override Task OnInitializedAsync()
    {
        InitializeMenuFile();
        InitializeMenuAccount();
        
        return base.OnInitializedAsync();
    }

    private void InitializeMenuFile()
    {
        var menuOptions = new List<MenuOptionRecord>();

        // Menu Option New
        {
            var menuOptionNewDotNetSolution = new MenuOptionRecord(
                ".NET Solution",
                MenuOptionKind.Other,
                OpenNewDotNetSolutionDialog);

            var menuOptionNew = new MenuOptionRecord(
                "New",
                MenuOptionKind.Other,
                SubMenu: new MenuRecord(
                    new[]
                    {
                        menuOptionNewDotNetSolution
                    }.ToImmutableArray()));
            
            menuOptions.Add(menuOptionNew);
        }
        
        // Menu Option Open
        {
            // TODO: Why do all the OnClicks have an async void lambda? Not quite sure what I was doing when I originally wrote this and should revisit this.
            
            var menuOptionOpenFile = new MenuOptionRecord(
                "File",
                MenuOptionKind.Other,
                async () => 
                    await EditorState.ShowInputFileAsync(
                        Dispatcher, 
                        TextEditorService,
                        CommonComponentRenderers,
                        FileSystemProvider));
        
            var menuOptionOpenDirectory = new MenuOptionRecord(
                "Directory",
                MenuOptionKind.Other,
                async () => 
                    await FolderExplorerState.ShowInputFileAsync(Dispatcher));
        
            var menuOptionOpenCSharpProject = new MenuOptionRecord(
                "C# Project - TODO: Adhoc Sln",
                MenuOptionKind.Other,
                async () => 
                    await EditorState.ShowInputFileAsync(
                        Dispatcher, 
                        TextEditorService,
                        CommonComponentRenderers,
                        FileSystemProvider));
        
            var menuOptionOpenDotNetSolution = new MenuOptionRecord(
                ".NET Solution",
                MenuOptionKind.Other,
                async () => 
                    await SolutionExplorerState.ShowInputFileAsync(
                        Dispatcher,
                        EnvironmentProvider));

            var menuOptionOpen = new MenuOptionRecord(
                "Open",
                MenuOptionKind.Other,
                SubMenu: new MenuRecord(
                    new []
                    {
                        menuOptionOpenFile,
                        menuOptionOpenDirectory,
                        menuOptionOpenCSharpProject,
                        menuOptionOpenDotNetSolution
                    }.ToImmutableArray()));
        
            menuOptions.Add(menuOptionOpen);
        }
        
        _menuFile = new MenuRecord(menuOptions.ToImmutableArray());
    }
    
    private void InitializeMenuAccount()
    {
        var menuOptions = new List<MenuOptionRecord>();

        // Menu Option Login
        {
            var menuOptionLogin = new MenuOptionRecord(
                "Login",
                MenuOptionKind.Other,
                WidgetRendererType: typeof(LoginFormDisplay));
            
            menuOptions.Add(menuOptionLogin);
        }
        
        _menuAccount = new MenuRecord(menuOptions.ToImmutableArray());
    }

    private void AddActiveDropdownKey(DropdownKey dropdownKey)
    {
        Dispatcher.Dispatch(
            new DropdownsState.AddActiveAction(
                dropdownKey));
    }
    
    /// <summary>
    /// TODO: Make this method abstracted into a component that takes care of the UI to show the dropdown and to restore focus when menu closed
    /// </summary>
    private void RestoreFocusToButtonDisplayComponentFile()
    {
        _buttonDisplayComponentFile?.ButtonElementReference?
            .FocusAsync();
    }
    
    /// <summary>
    /// TODO: Make this method abstracted into a component that takes care of the UI to show the dropdown and to restore focus when menu closed
    /// </summary>
    private void RestoreFocusToButtonDisplayComponentAccount()
    {
        _buttonDisplayComponentAccount?.ButtonElementReference?
            .FocusAsync();
    }

    private void OpenNewDotNetSolutionDialog()
    {
        var dialogRecord = new DialogRecord(
            DialogKey.NewDialogKey(), 
            "New .NET Solution",
            typeof(DotNetSolutionFormDisplay),
            null)
        {
            IsResizable = true
        };
        
        Dispatcher.Dispatch(
            new DialogRecordsCollection.RegisterAction(
                dialogRecord));
    }
}