using BlazorStudio.ClassLib.CommonComponents;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.File;

public partial class FileFormDisplay 
    : ComponentBase, IFileFormRendererType
{
    [Parameter, EditorRequired]
    public string FileName { get; set; } = string.Empty;

    private string? _previousFileNameParameter;

    private string _fileName = string.Empty;
    
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
}