using Microsoft.AspNetCore.Components.Web;

namespace BlazorStudio.ClassLib.Store.DragCase;

public record SetDragStateAction(bool IsDisplayed, MouseEventArgs MouseEventArgs);