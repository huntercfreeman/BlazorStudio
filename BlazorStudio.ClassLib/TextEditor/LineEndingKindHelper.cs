namespace BlazorStudio.ClassLib.TextEditor;

public static class LineEndingKindHelper
{
    public static LineEndingKind GetLineEndingKind(char character)
    {
        return character switch
        {
            '\r' => LineEndingKind.CarriageReturn,
            '\n' => LineEndingKind.NewLine,
            _ => throw new ApplicationException($"Unrecognized line ending {nameof(character)}: {character}")
        };
    }
}