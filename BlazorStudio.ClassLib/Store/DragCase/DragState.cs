using Fluxor;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorStudio.ClassLib.Store.DragCase;

[FeatureState]
public record DragState(bool IsDisplayed, MouseEventArgs? MouseEventArgs)
{
    public DragState() : this(false, null)
    {

    }
}