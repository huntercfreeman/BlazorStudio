using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.Menu;

public class MenuOptionWidgetParameters
{
    public MenuOptionWidgetParameters(
        Func<bool, Task> setShouldDisplayWidgetAsync, 
        ElementReference? menuOptionDisplayElementReference)
    {
        SetShouldDisplayWidgetAsync = setShouldDisplayWidgetAsync;
        MenuOptionDisplayElementReference = menuOptionDisplayElementReference;
    }

    /// <summary>
    /// Invoke with false to hide the widget -- perhaps on submit of a form.
    /// <br/><br/>
    /// Focus will be internally set back to the
    /// <see cref="MenuOptionDisplayElementReference"/>
    /// if this is invoked with false.
    /// </summary>
    public Func<bool, Task> SetShouldDisplayWidgetAsync { get; }
    public ElementReference? MenuOptionDisplayElementReference { get; }
}