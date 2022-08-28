using System.Collections.Immutable;

namespace ExperimentalTextEditor.ClassLib.TextEditor;

/// <summary>
/// The length of a row might horizontally go offscreen and not get rendered.
/// Therefore the end user's viewport is instead represented as a <see cref="TextPartition"/>
/// and that <see cref="TextPartition"/> has one or many <see cref="TextSpan"/> of which
/// represent the 'row' and may or may not be the 'entirety' of that row.
/// </summary>
public record TextSpan(ImmutableArray<TextCharacter> TextCharacters)
{
    /// <summary>
    /// <see cref="Start"/> is inclusive
    /// </summary>
    public int Start { get; set; }
    /// <summary>
    /// <see cref="End"/> is exclusive
    /// </summary>
    public int End { get; set; }
}