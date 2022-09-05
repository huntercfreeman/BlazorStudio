using System.Collections.Immutable;
using BlazorStudio.ClassLib.TextEditor.Character;

namespace BlazorStudio.ClassLib.TextEditor;

/// <summary>
/// The length of a row might horizontally go offscreen and not get rendered.
/// Therefore the end user's viewport is instead represented as a <see cref="TextPartition"/>
/// and that <see cref="TextPartition"/> has one or many <see cref="TextCharacterSpan"/> of which
/// represent the 'row' and may or may not be the 'entirety' of that row.
/// </summary>
public class TextCharacterSpan
{
    /// <summary>
    /// <see cref="Start"/> is inclusive
    /// </summary>
    public int Start { get; set; }
    /// <summary>
    /// <see cref="End"/> is exclusive
    /// </summary>
    public int End { get; set; }
    /// <summary>
    /// If this <see cref="TextCharacterSpan" /> represents the content of a
    /// single row that row's index is included as <see cref="RowIndex"/>.
    /// </summary>
    public int? RowIndex { get; set; }
    public List<TextCharacter> TextCharacters { get; set; } = new();
    public string GetText => new string(TextCharacters
        .Select(x => x.Value)
        .ToArray());
}