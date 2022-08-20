using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.CustomEvents;

[EventHandler("oncustomkeydown", typeof(CustomKeyDown), enableStopPropagation: true, enablePreventDefault: true)]
public static class EventHandlers
{
}