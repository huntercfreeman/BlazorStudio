namespace BlazorStudio.ClassLib.Store.PlainTextEditorCase;

/// <summary>
/// This allows the selection to be relative to the document
/// and span multiple rows
/// </summary>
public record SelectionSpanRecord
{
    /// <summary>
    /// InclusiveStartingDocumentTextIndex is the total of the
    /// lengths of all previous rows plus the column in the current row
    /// and is the first column to highlight.
    /// </summary>
    public int InclusiveStartingDocumentTextIndex { get; init; }
    public int ExclusiveEndingDocumentTextIndex { get; init; }

    public SelectionDirection SelectionDirection =>
        (ExclusiveEndingDocumentTextIndex - InclusiveStartingDocumentTextIndex) < 0
            ? SelectionDirection.Left
            : SelectionDirection.Right;
}