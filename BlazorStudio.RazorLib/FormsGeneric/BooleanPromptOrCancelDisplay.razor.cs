using BlazorALaCarte.Shared.Keyboard;
using BlazorALaCarte.Shared.Menu;
using BlazorStudio.ClassLib.CommonComponents;
using BlazorStudio.ClassLib.FileSystem.Classes;
using BlazorStudio.ClassLib.FileSystem.Interfaces;
using BlazorStudio.RazorLib.Button;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorStudio.RazorLib.FormsGeneric;

public partial class BooleanPromptOrCancelDisplay : ComponentBase, IBooleanPromptOrCancelRendererType
{
    [CascadingParameter]
    public MenuOptionWidgetParameters? MenuOptionWidgetParameters { get; set; }
    
    [Parameter, EditorRequired]
    public bool IncludeCancelOption { get; set; }
    [Parameter, EditorRequired]
    public string Message { get; set; } = null!;
    [Parameter, EditorRequired]
    public string? AcceptOptionTextOverride { get; set; }
    [Parameter, EditorRequired]
    public string? DeclineOptionTextOverride { get; set; }
    [Parameter, EditorRequired]
    public Action OnAfterAcceptAction { get; set; } = null!;
    [Parameter, EditorRequired]
    public Action OnAfterDeclineAction { get; set; } = null!;
    [Parameter, EditorRequired]
    public Action OnAfterCancelAction { get; set; } = null!;
    
    private ButtonDisplay? _declineButtonDisplay;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            if (_declineButtonDisplay?.ButtonElementReference is not null)
            {
                await _declineButtonDisplay.ButtonElementReference.Value.FocusAsync();
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
}