using BlazorStudio.ClassLib.CommonComponents;
using BlazorStudio.ClassLib.FileSystem.Interfaces;
using BlazorStudio.ClassLib.Keyboard;
using BlazorStudio.RazorLib.Button;
using BlazorStudio.RazorLib.Menu;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorStudio.RazorLib.File;

public partial class DeleteFileFormDisplay 
    : ComponentBase, IDeleteFileFormRendererType
{
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

    protected override Task OnParametersSetAsync()
    {
        if (_previousAbsoluteFilePath is null ||
            _previousAbsoluteFilePath.GetAbsoluteFilePathString() != 
            AbsoluteFilePath.GetAbsoluteFilePathString())
        {
            _countOfImmediateChildren = null;
            
            _previousAbsoluteFilePath = AbsoluteFilePath;

            if (AbsoluteFilePath.IsDirectory)
            {
                _countOfImmediateChildren = Directory
                    .EnumerateFileSystemEntries(
                        AbsoluteFilePath.GetAbsoluteFilePathString())
                    .Count();
            }
        }
        
        return base.OnParametersSetAsync();
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