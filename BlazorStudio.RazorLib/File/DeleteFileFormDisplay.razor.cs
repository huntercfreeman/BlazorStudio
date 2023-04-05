using BlazorCommon.RazorLib.Keyboard;
using BlazorCommon.RazorLib.Menu;
using BlazorStudio.ClassLib.ComponentRenderers;
using BlazorStudio.ClassLib.FileSystem.Interfaces;
using BlazorStudio.RazorLib.Button;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorStudio.RazorLib.File;

public partial class DeleteFileFormDisplay 
    : ComponentBase, IDeleteFileFormRendererType
{
    [Inject]
    private IFileSystemProvider FileSystemProvider { get; set; } = null!;
    
    [CascadingParameter]
    public MenuOptionWidgetParameters? MenuOptionWidgetParameters { get; set; }
    
    [Parameter, EditorRequired]
    public IAbsoluteFilePath AbsoluteFilePath { get; set; } = null!;
    [Parameter, EditorRequired]
    public bool IsDirectory { get; set; }
    [Parameter, EditorRequired]
    public Action<IAbsoluteFilePath> OnAfterSubmitAction { get; set; } = null!;
    
    private IAbsoluteFilePath? _previousAbsoluteFilePath;

    private int? _countOfImmediateChildren;
    private ButtonDisplay? _cancelButtonDisplay;

    protected override async Task OnParametersSetAsync()
    {
        if (_previousAbsoluteFilePath is null ||
            _previousAbsoluteFilePath.GetAbsoluteFilePathString() != 
            AbsoluteFilePath.GetAbsoluteFilePathString())
        {
            _countOfImmediateChildren = null;
            
            _previousAbsoluteFilePath = AbsoluteFilePath;

            if (AbsoluteFilePath.IsDirectory)
            {
                _countOfImmediateChildren = (await FileSystemProvider.Directory
                    .EnumerateFileSystemEntriesAsync(
                        AbsoluteFilePath.GetAbsoluteFilePathString()))
                    .Count();
            }
        }
        
        await base.OnParametersSetAsync();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            if (MenuOptionWidgetParameters is not null && 
                _cancelButtonDisplay?.ButtonElementReference is not null)
            {
                await _cancelButtonDisplay.ButtonElementReference.Value.FocusAsync();
            }
        }
        
        await base.OnAfterRenderAsync(firstRender);
    }

    private async Task HandleOnKeyDown(KeyboardEventArgs keyboardEventArgs)
    {
        if (MenuOptionWidgetParameters is not null)
        {
            if (keyboardEventArgs.Key == KeyboardKeyFacts.MetaKeys.ESCAPE)
            {
                await MenuOptionWidgetParameters.HideWidgetAsync.Invoke();
            }
        }
    }

    private async Task DeleteFileOnClick()
    {
        var localAbsoluteFilePath = AbsoluteFilePath;
        
        if (MenuOptionWidgetParameters is not null)
        {
            await MenuOptionWidgetParameters.CompleteWidgetAsync.Invoke(
                () => OnAfterSubmitAction.Invoke(localAbsoluteFilePath));
        }
    }

    private async Task CancelOnClick()
    {
        if (MenuOptionWidgetParameters is not null)
        {
            await MenuOptionWidgetParameters.HideWidgetAsync.Invoke();
        }
    }
}