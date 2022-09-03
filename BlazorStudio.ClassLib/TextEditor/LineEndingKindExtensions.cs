namespace BlazorStudio.ClassLib.TextEditor;

public static class LineEndingKindExtensions
{
    /// <summary>
    /// In order to not override the ToString() method in a possibly unexpected way <see cref="AsCharacters"/> was made
    /// to convert a <see cref="LineEndingKind"/> to its character(s) representation.
    /// <br/><br/>
    /// Example: <see cref="LineEndingKind.NewLine"/> would return '\n'
    /// </summary>
    public static string AsCharacters(this LineEndingKind lineEndingKind)
    {
        return lineEndingKind switch
        {
            LineEndingKind.CarriageReturn => "\r",
            LineEndingKind.NewLine => "\n",
            LineEndingKind.CarriageReturnNewLine => "\r\n",
            LineEndingKind.StartOfFile or LineEndingKind.EndOfFile => string.Empty,
            _ => throw new ApplicationException($"Unexpected {nameof(LineEndingKind)} of: {lineEndingKind}")
        };
    }
}