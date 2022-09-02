using BlazorStudio.RazorLib.CustomEvents;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.ClassLib.CustomEvents;

[EventHandler("oncustomkeydown", typeof(CustomKeyDown), enableStopPropagation: true, enablePreventDefault: true)]
[EventHandler("oncustomclick", typeof(CustomOnClick), enableStopPropagation: true, enablePreventDefault: true)]
public static class EventHandlers
{
}
