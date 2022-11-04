using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.Menu;

public class MenuOptionWidgetParameters
{
    public MenuOptionWidgetParameters(
        Func<bool, Task> setShouldDisplayWidgetAsync)
    {
        SetShouldDisplayWidgetAsync = setShouldDisplayWidgetAsync;
    }

    /// <summary>
    /// Invoke with false to hide the widget -- perhaps on submit of a form.
    /// <br/><br/>
    /// Focus will be internally set back to the
    /// menu option display if this is invoked with false.
    /// </summary>
    public Func<bool, Task> SetShouldDisplayWidgetAsync { get; }
}