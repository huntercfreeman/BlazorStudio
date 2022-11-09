using Microsoft.AspNetCore.Components;

namespace BlazorStudio.ClassLib.CustomEvents;

[EventHandler("onbskeydown", typeof(BsKeyDownEventArgs), enableStopPropagation: true, enablePreventDefault: true)]
public static class EventHandlers
{
}