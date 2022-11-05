using BlazorStudio.ClassLib.CommonComponents;
using BlazorStudio.ClassLib.Keyboard;
using BlazorStudio.RazorLib.Menu;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorStudio.RazorLib.File;

public partial class FileFormDisplay 
    : ComponentBase, IFileFormRendererType
{
    [CascadingParameter]
    public MenuOptionWidgetParameters? MenuOptionWidgetParameters { get; set; }
    
    [Parameter, EditorRequired]
    public string FileName { get; set; } = string.Empty;
    [Parameter, EditorRequired]
    public Action<string> OnAfterSubmitAction { get; set; } = null!;
    [Parameter]
    public bool IsDirectory { get; set; }

    private string? _previousFileNameParameter;

    private string _fileName = string.Empty;

    private string PlaceholderText => IsDirectory
        ? "Directory name"
        : "File name";
    
    public string InputFileName => _fileName;

    protected override Task OnParametersSetAsync()
    {
        if (_previousFileNameParameter is null ||
            _previousFileNameParameter != FileName)
        {
            _previousFileNameParameter = FileName;
            _fileName = FileName;
        }
        
        return base.OnParametersSetAsync();
    }

    private async Task HandleOnKeyDown(KeyboardEventArgs keyboardEventArgs)
    {
        if (MenuOptionWidgetParameters is not null)
        {
            if (keyboardEventArgs.Key == KeyboardKeyFacts.MetaKeys.ESCAPE)
            {
                await MenuOptionWidgetParameters.HideWidgetAsync.Invoke();
            }
            else if (keyboardEventArgs.Code == KeyboardKeyFacts.WhitespaceCodes.ENTER_CODE)
            {
                await MenuOptionWidgetParameters.CompleteWidgetAsync.Invoke(
                    () => OnAfterSubmitAction.Invoke(_fileName));
            }
        }
    }
}