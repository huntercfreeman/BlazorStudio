using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.VirtualizeComponents;

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