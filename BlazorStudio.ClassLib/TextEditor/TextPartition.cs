using System.Collections.Immutable;
using ExperimentalTextEditor.ClassLib.TextEditor.IndexWrappers;

namespace ExperimentalTextEditor.ClassLib.TextEditor;

public record TextPartition(RectangularCoordinates RectangularCoordinates, ImmutableArray<TextSpan> TextSpanRows);