// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
//
// Using disable UnusedAutoPropertyAccessor.Global because
// CustomKeyDown is used by Blazor's internal event logic
// and I don't want to touch anything regarding this.
namespace BlazorStudio.ClassLib.CustomEvents;

public class CustomKeyDown : EventArgs
{
    public string Key { get; set; } = null!;
    public string Code { get; set; } = null!;
    public bool CtrlWasPressed { get; set; }
    public bool ShiftWasPressed { get; set; }
    public bool AltWasPressed { get; set; }
}