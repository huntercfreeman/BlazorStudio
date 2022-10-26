using Microsoft.AspNetCore.Components;

namespace BlazorStudio.ClassLib.CustomEvents;

[EventHandler("oncustomkeydown", typeof(CustomKeyDown), true, true)]
[EventHandler("oncustomclick", typeof(CustomOnClick), true, true)]
public static class EventHandlers
{
}