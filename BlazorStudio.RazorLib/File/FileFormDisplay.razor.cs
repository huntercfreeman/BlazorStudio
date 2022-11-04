﻿using BlazorStudio.ClassLib.CommonComponents;
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

    private string? _previousFileNameParameter;

    private string _fileName = string.Empty;

    private string _aaa = string.Empty;
    
    public string Aaa
    {
        get => _aaa;
        set
        {
            _aaa = value;
        }
    }
    
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

    public string InputFileName => _fileName;

    private async Task HandleOnKeyDown(KeyboardEventArgs keyboardEventArgs)
    {
        if (MenuOptionWidgetParameters is not null)
        {
            if (keyboardEventArgs.Key == KeyboardKeyFacts.MetaKeys.ESCAPE)
            {
                await MenuOptionWidgetParameters.SetShouldDisplayWidgetAsync.Invoke(false);
            }
        }
    }
}