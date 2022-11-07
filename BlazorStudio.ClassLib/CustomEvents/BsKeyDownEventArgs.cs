namespace BlazorStudio.ClassLib.CustomEvents;

/// <summary>
/// The <see cref="BsKeyDownEventArgs"/> class
/// is used as a custom Blazor event.
/// <br/><br/>
/// The purpose is to conditionally
/// stopPropagation on a @onkeydown event.
/// <br/><br/>
/// In specific this will stopPropagation
/// on all @onkeydown events where the
/// e.code !== "Tab"
/// <br/><br/>
/// The need for this arises due to Blazor components
/// that have a scroll bar, keyboard events, and are focusable.
/// <br/><br/>
/// The user needs to be able to 'tab target' to the next
/// focusable element. The user must not be stuck at any point unable
/// to 'tab target and this event fixes the issue. 
/// </summary>
public class BsKeyDownEventArgs : EventArgs
{
    public string Key { get; set; }
    public string Code { get; set; }
    public bool CtrlWasPressed { get; set; }
    public bool ShiftWasPressed { get; set; }
    public bool AltWasPressed { get; set; }
}