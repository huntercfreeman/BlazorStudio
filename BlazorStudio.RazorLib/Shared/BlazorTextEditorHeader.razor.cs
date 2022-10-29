using System.Collections.Immutable;
using BlazorStudio.ClassLib.Menu;
using BlazorStudio.ClassLib.Store.DialogCase;
using BlazorStudio.ClassLib.Store.DropdownCase;
using BlazorStudio.ClassLib.Store.FolderExplorerCase;
using BlazorStudio.ClassLib.Store.InputFileCase;
using BlazorStudio.RazorLib.Button;
using BlazorStudio.RazorLib.InputFile;
using Fluxor;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.Shared;

public partial class BlazorTextEditorHeader : ComponentBase
{
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;
    
    private DropdownKey _dropdownKeyFileDropdown = DropdownKey.NewDropdownKey();
    private MenuRecord _fileMenu = new(ImmutableArray<MenuOptionRecord>.Empty);
    private ButtonDisplay? _fileButtonDisplay;

    protected override Task OnInitializedAsync()
    {
        _fileMenu = new MenuRecord(
            new []
            {
                GetMenuOptionNew(),
                GetMenuOptionOpen()
            }.ToImmutableArray());
        
        return base.OnInitializedAsync();
    }

    private MenuOptionRecord GetMenuOptionNew()
    {
        return new MenuOptionRecord(
            "New");
    }
    
    private MenuOptionRecord GetMenuOptionOpen()
    {
        var openFile = new MenuOptionRecord(
            "File",
            ShowInputFileDialog);
        
        var openCSharpProject = new MenuOptionRecord(
            "C# Project");
        
        var openDotNetSolution = new MenuOptionRecord(
            ".NET Solution");

        return new MenuOptionRecord(
            "Open",
            SubMenu: new MenuRecord(
                new []
                {
                    openFile,
                    openCSharpProject,
                    openDotNetSolution
                }.ToImmutableArray()));
    }

    private void AddActiveFileDropdown()
    {
        Dispatcher.Dispatch(new AddActiveDropdownKeyAction(_dropdownKeyFileDropdown));
    }

    private void ShowInputFileDialog()
    {
        Dispatcher.Dispatch(
            new InputFileState.RequestInputFileStateFormAction(
                afp =>
                {
                    Dispatcher.Dispatch(
                        new SetFolderExplorerStateAction(afp));
                    
                    return Task.CompletedTask;
                },
                afp =>
                {
                    if (afp is null ||
                        !afp.IsDirectory)
                    {
                        return Task.FromResult(false);
                    }
                    
                    return Task.FromResult(true);
                }));
    }
    
    /// <summary>
    /// TODO: Make this method abstracted into a component that takes care of the UI to show the dropdown and to restore focus when menu closed
    /// </summary>
    private void RestoreFocusToFileButton()
    {
        _fileButtonDisplay?.ButtonElementReference?
            .FocusAsync();
    }
}