using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BlazorStudio.RazorLib.VirtualizeComponentExperiments;

/// <summary>
/// The Virtualization is dependent on knowing the width and height
/// of each item rendered.
/// <br/>--<br/>
/// This Blazor component contains the logic to render a single <see cref="TItem"/>
/// using the <see cref="ItemRenderFragment"/> and measure the rendered
/// content's dimensions. The dimensions are stored as <see cref="VirtualizeItemDimensions"/>
/// </summary>
public partial class MeasureDimensionsForVirtualization<TItem> : ComponentBase
{
    /// <summary>
    /// The JavaScript interop calls are performed through
    /// the <see cref="IJSRuntime"/> interface that Microsoft provides.
    /// </summary>
    [Inject] 
    private IJSRuntime JsRuntime { get; set; } = null!;
    
    /// <summary>
    /// The <see cref="TItem"/> to have its rendered content measured.
    /// </summary>
    [Parameter, EditorRequired] 
    public TItem? Item { get; set; }
    /// <summary>
    /// The <see cref="RenderFragment"/> that will be used to render
    /// the <see cref="TItem"/> for measurement of its rendered content.
    /// </summary>
    [Parameter, EditorRequired] 
    public RenderFragment<TItem> ItemRenderFragment { get; set; } = null!;

    /// <summary>
    /// After measuring the Dimensions of the result from <see cref="ItemRenderFragment"/>
    /// this <see cref="EventCallback"/> is invoked to notify the Parent component the measurement was taken.
    /// </summary>
    [Parameter, EditorRequired]
    public EventCallback<VirtualizeItemDimensions> OnAfterMeasurementTakenEventCallback { get; set; }

    /// <summary>
    /// An <see cref="ElementReference"/> to be used along with the
    /// injected <see cref="JsRuntime"/> to locate the <see cref="TItem"/>
    /// once it has been rendered with the <see cref="ItemRenderFragment"/>
    /// </summary>
    private ElementReference? _virtualizeItemLocatorElementReference;

    /// <summary>
    /// After the initial render the rendered content from
    /// <see cref="ItemRenderFragment"/> will be available for measurement.
    /// <br/>--<br/>
    /// Therefore, if <see cref="firstRender"/> then take the measurements.
    /// <br/>--<br/>
    /// After measuring invoke an <see cref="OnAfterMeasurementTakenEventCallback"/> to notify
    /// the Parent component the measurement was taken.
    /// </summary>
    /// <returns></returns>
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            var virtualizeItemDimensions = await JsRuntime
                .InvokeAsync<VirtualizeItemDimensions>(
                    "plainTextEditor.getVirtualizeItemDimensions",
                    _virtualizeItemLocatorElementReference);

            await OnAfterMeasurementTakenEventCallback.InvokeAsync(virtualizeItemDimensions);
        }
        
        await base.OnAfterRenderAsync(firstRender);
    }
}