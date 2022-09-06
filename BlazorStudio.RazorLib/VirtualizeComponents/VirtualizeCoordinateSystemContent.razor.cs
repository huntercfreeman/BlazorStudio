using System.Collections.Immutable;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.VirtualizeComponents;

public partial class VirtualizeCoordinateSystemContent<T> : ComponentBase
{
    [Parameter, EditorRequired]
    public RenderFragment<VirtualizeCoordinateSystemEntry<T>> ChildContent { get; set; } = null!;
    [Parameter, EditorRequired]
    public ImmutableArray<VirtualizeCoordinateSystemEntry<T>> ItemsToRender { get; set; }
}