using System.Collections.Immutable;
using BlazorStudio.ClassLib.TextEditor.Character;
using BlazorStudio.ClassLib.TextEditor.IndexWrappers;

namespace BlazorStudio.ClassLib.TextEditor;

public record RectangularCoordinateMap
{
    private readonly Func<ImmutableArray<TextCharacter>> _getContentFunc;
    private readonly Dictionary<RectangularCoordinates, ImmutableArray<TextPartition>> _cachedTextPartitions = new();

    public RectangularCoordinateMap(Func<ImmutableArray<TextCharacter>> getContentFunc)
    {
        _getContentFunc = getContentFunc;
    }
}