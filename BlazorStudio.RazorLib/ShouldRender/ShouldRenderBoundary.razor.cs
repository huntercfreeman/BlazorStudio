using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.ShouldRender;

public partial class ShouldRenderBoundary : ComponentBase
{
    private IsFirstShouldRenderValue _isFirstShouldRenderValue = new(true);
    [Parameter]
    [EditorRequired]
    public Func<IsFirstShouldRenderValue, bool> ShouldRenderFunc { get; set; } = null!;
    [Parameter]
    [EditorRequired]
    public RenderFragment ChildContent { get; set; } = null!;

    protected override bool ShouldRender()
    {
        return true;
/*
        var shouldRender = ShouldRenderFunc.Invoke(_isFirstShouldRenderValue);

        _isFirstShouldRenderValue = new IsFirstShouldRenderValue(false);

        return shouldRender;
*/
    }

    public record IsFirstShouldRenderValue(bool IsFirstShouldRender);
}