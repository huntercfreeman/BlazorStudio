using Microsoft.AspNetCore.Components;

namespace BlazorStudio.ClassLib.CustomEvents;

[EventHandler("oncustomkeydown", typeof(CustomKeyDownEventArgs), enableStopPropagation: true, enablePreventDefault: true)]
public static class EventHandlers
{
}