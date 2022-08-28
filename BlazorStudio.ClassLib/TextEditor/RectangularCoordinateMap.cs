using System.Collections.Immutable;
using BlazorStudio.ClassLib.TextEditor.IndexWrappers;

namespace BlazorStudio.ClassLib.TextEditor;

public record RectangularCoordinateMap(
    ImmutableDictionary<RectangularCoordinates, ImmutableArray<TextPartition>> TextPartitions)
{
}