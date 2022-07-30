namespace BlazorStudio.ClassLib.FileSystemApi;

/// <summary>
/// A positive <see cref="ChangeAmount"/> is interpreted as a character(s) insertion at <see cref="Index"/>
/// <br/><br/>
/// A negative <see cref="ChangeAmount"/> is interpreted as a character(s) remove operation starting
/// inclusively with <see cref="Index"/> and spanning the length of <see cref="ChangeAmount"/>
/// </summary>
public abstract record DisplacementValue(int Index, int ChangeAmount)
{
    public abstract DisplacementKind DisplacementKind { get; }
}