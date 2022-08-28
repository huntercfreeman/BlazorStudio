using System.Collections.Immutable;
using BlazorStudio.ClassLib.TextEditor.IndexWrappers;

namespace BlazorStudio.ClassLib.TextEditor;

public record TextPartition(RectangularCoordinates RectangularCoordinates, ImmutableArray<TextSpan> TextSpanRows);