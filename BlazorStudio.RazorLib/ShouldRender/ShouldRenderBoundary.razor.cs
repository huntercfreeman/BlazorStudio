using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.ShouldRender;

public partial class ShouldRenderBoundary : ComponentBase
{
    [Parameter, EditorRequired]
    public Func<IsFirstShouldRenderValue, bool> ShouldRenderFunc { get; set; } = null!;
    [Parameter, EditorRequired]
    public RenderFragment ChildContent { get; set; } = null!;

    private IsFirstShouldRenderValue _isFirstShouldRenderValue = new(true);
    
    protected override bool ShouldRender()
    {
        var shouldRender = ShouldRenderFunc.Invoke(_isFirstShouldRenderValue);

        _isFirstShouldRenderValue = new IsFirstShouldRenderValue(false);

        return shouldRender;
    }

    public record IsFirstShouldRenderValue(bool IsFirstShouldRender);
}