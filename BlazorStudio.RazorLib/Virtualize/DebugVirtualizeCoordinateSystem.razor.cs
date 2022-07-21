using System.Collections.Immutable;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BlazorStudio.RazorLib.Virtualize;

public partial class DebugVirtualizeCoordinateSystem<T> : IDisposable
{
    [Parameter]
    public Func<VirtualizeCoordinateSystem<T>> GetVirtualizeCoordinateSystemComponentFunc { get; set; } = null!;

    private VirtualizeCoordinateSystem<T>? _virtualizeCoordinateSystemComponent;

    private void SetTargetComponent()
    {
        if (_virtualizeCoordinateSystemComponent is not null)
        {
            _virtualizeCoordinateSystemComponent._componentStateChanged -= VirtualizeCoordinateSystemComponentOn_componentStateChanged;
        }

        _virtualizeCoordinateSystemComponent = GetVirtualizeCoordinateSystemComponentFunc();

        if (_virtualizeCoordinateSystemComponent is not null)
        {
            _virtualizeCoordinateSystemComponent._componentStateChanged +=
                VirtualizeCoordinateSystemComponentOn_componentStateChanged;
        }
    }

    private async void VirtualizeCoordinateSystemComponentOn_componentStateChanged(object? sender, EventArgs e)
    {
        await InvokeAsync(StateHasChanged);
    }

    public void Dispose()
    {
        if (_virtualizeCoordinateSystemComponent is not null)
        {
            _virtualizeCoordinateSystemComponent._componentStateChanged -= VirtualizeCoordinateSystemComponentOn_componentStateChanged;
        }
    }
}