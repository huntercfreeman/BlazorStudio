namespace BlazorStudio.ClassLib.Store.PlainTextEditorCase;

/// <summary>
/// This allows the selection to be relative to the document
/// and span multiple rows
/// </summary>
public class SelectionSpan
{
    /// <summary>
    /// InclusiveStartingDocumentTextIndex is the total of the
    /// lengths of all previous rows plus the column in the current row
    /// and is the first column to highlight.
    /// </summary>
    public int InclusiveStartingDocumentTextIndex { get; set; }
    /// <summary>
    /// A positive displacement is to say from the marker
    /// highlight additionally the next {x} column
    /// 
    /// A negative displacement is to say from the marker
    /// highlight additionally the previous {x} column
    /// </summary>
    public int OffsetDisplacement { get; set; }
}