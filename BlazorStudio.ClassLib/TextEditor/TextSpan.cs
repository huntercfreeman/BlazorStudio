using System.Collections.Immutable;
using BlazorStudio.ClassLib.TextEditor.Character;

namespace BlazorStudio.ClassLib.TextEditor;

/// <summary>
/// The length of a row might horizontally go offscreen and not get rendered.
/// Therefore the end user's viewport is instead represented as a <see cref="TextPartition"/>
/// and that <see cref="TextPartition"/> has one or many <see cref="TextSpan"/> of which
/// represent the 'row' and may or may not be the 'entirety' of that row.
/// </summary>
public record TextSpan
{
    /// <summary>
    /// <see cref="Start"/> is inclusive
    /// </summary>
    public int Start { get; init; }
    /// <summary>
    /// <see cref="End"/> is exclusive
    /// </summary>
    public int End { get; init; }
    public ImmutableArray<TextCharacter> TextCharacters { get; init; }
}