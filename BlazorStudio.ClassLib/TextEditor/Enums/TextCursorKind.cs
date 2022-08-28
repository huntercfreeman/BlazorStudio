namespace ExperimentalTextEditor.ClassLib.TextEditor.Enums;

/// <summary>
/// <see cref="TextCursorKind.Beam"/> is intended for text insertion via typing.
/// <br/><br/>
/// <see cref="TextCursorKind.Block"/> is intended for a 'second editor mode' indicator.
/// <br/><br/>
/// <see cref="TextCursorKind.Replace"/> is intended for when the 'insert' key is toggled to replace the text after where the insertion occurred.
/// <br/><br/>
/// <see cref="TextCursorKind.AlternateBeam"/> No intended usage as of right now but I aim to make the Editor very (Extendable / customizable) and will likely use this for that.
/// <br/><br/>
/// <see cref="TextCursorKind.AlternateBlock"/> No intended usage as of right now but I aim to make the Editor very (Extendable / customizable) and will likely use this for that.
/// <br/><br/>
/// <see cref="TextCursorKind.AlternateReplace"/> No intended usage as of right now but I aim to make the Editor very (Extendable / customizable) and will likely use this for that.
/// <br/><br/>
/// </summary>
public enum TextCursorKind
{
    Beam,
    Block,
    Replace,
    AlternateBeam,
    AlternateBlock,
    AlternateReplace
}