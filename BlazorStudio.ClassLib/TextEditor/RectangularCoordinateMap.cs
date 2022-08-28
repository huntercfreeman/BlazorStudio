using System.Collections.Immutable;
using ExperimentalTextEditor.ClassLib.TextEditor.IndexWrappers;

namespace ExperimentalTextEditor.ClassLib.TextEditor;

public record RectangularCoordinateMap(
    ImmutableDictionary<RectangularCoordinates, ImmutableArray<TextPartition>> TextPartitions)
{
}