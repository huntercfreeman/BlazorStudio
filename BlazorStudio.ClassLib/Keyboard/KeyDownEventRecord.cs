namespace BlazorStudio.ClassLib.Keyboard;

public record KeyDownEventRecord(string Key,
                                   string Code,
                                   bool CtrlWasPressed,
                                   bool ShiftWasPressed,
                                   bool AltWasPressed)
{
}
