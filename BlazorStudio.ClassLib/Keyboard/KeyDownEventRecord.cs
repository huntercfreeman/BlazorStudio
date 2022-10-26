namespace BlazorStudio.ClassLib.Keyboard;

/// <summary>
///     <see cref="IsForced" /> is used when initializing the contents of an opened file as it is important to not mark the
///     initialization writes as edits
/// </summary>
public record KeyDownEventRecord(string Key,
    string Code,
    bool CtrlWasPressed,
    bool ShiftWasPressed,
    bool AltWasPressed,
    bool IsForced = false)
{
}