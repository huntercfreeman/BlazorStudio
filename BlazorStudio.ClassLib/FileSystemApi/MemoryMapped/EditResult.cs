namespace BlazorStudio.ClassLib.FileSystemApi.MemoryMapped;

/// <summary>
/// As the user types in the virtual file the content of the virtual file strays further from
/// the physical file.
///
/// A list of edits to the virtual file is therefore kept.
///
/// This allows one to avoid storing the entirety of the physical file virtually in memory.
///
/// One solely reads from the physical file what rows, and characters are requested into memory.
///
/// Then the list of edits is enumerated such that the user sees a version that reflects their edits.
///
/// Once the user saves the file the list of edits are cleared.
///
/// <see cref="EditResult"/> comes from enumerating a list of edits
/// </summary>
public class EditResult
{
    public EditResult(List<string> contentRows,
        List<long> virtualCharacterIndexMarkerForStartOfARow, 
        List<DisplacementValue> displacementTimeline, 
        int accumulatedRowDisplacement)
    {
        ContentRows = contentRows;
        DisplacementTimeline = displacementTimeline;
        AccumulatedRowDisplacement = accumulatedRowDisplacement;
        VirtualCharacterIndexMarkerForStartOfARow = virtualCharacterIndexMarkerForStartOfARow;
    }
    
    /// <summary>
    /// This is the content of the virtual file (which is likely to be a small portion of the physical file).
    /// </summary>
    public List<string> ContentRows { get; }
    /// <summary>
    /// <see cref="CharacterDisplacementByRow"/> allows for tracking of how much the difference between
    /// the virtual file contents and the physical file contents.
    /// <br/><br/>
    /// Index 0 is the first displacement that occurred to the document
    /// </summary>
    public List<DisplacementValue> DisplacementTimeline { get; }
    /// <summary>
    /// As edits are applied to a document this variable will keep a running total of row displacements
    /// to avoid recalculating row displacement every individual edit.
    ///
    /// Then the character displacement can be used to get 'where()' actual row
    /// </summary>
    public int AccumulatedRowDisplacement { get; set;  }
    public List<long> VirtualCharacterIndexMarkerForStartOfARow { get; }
}