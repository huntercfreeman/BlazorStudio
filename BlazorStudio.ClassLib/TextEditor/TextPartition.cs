using System.Collections.Immutable;
using BlazorStudio.ClassLib.Sequence;
using BlazorStudio.ClassLib.TextEditor.IndexWrappers;

namespace BlazorStudio.ClassLib.TextEditor;

public record TextPartition(
    TextEditorLink TextEditorLink, 
    RectangularCoordinates RectangularCoordinates, 
    List<TextCharacterSpan> TextSpanRows,
    SequenceKey SequenceKey);