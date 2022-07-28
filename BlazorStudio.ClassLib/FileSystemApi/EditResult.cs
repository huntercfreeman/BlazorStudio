namespace BlazorStudio.ClassLib.FileSystemApi;

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
    public EditResult(List<string> content, List<(int rowIndex, int characterIndexOfInsertion, int amountOfCharactersInserted)> characterDisplacementByRow, List<(int insertionPoint, int amountOfRowsInserted)> rowDisplacement)
    {
        Content = content;
        CharacterDisplacementByRow = characterDisplacementByRow;
        RowDisplacement = rowDisplacement;
    }
    
    /// <summary>
    /// This is the content of the virtual file (which is likely to be a small portion of the physical file).
    /// </summary>
    public List<string> Content { get; }
    /// <summary>
    /// <see cref="CharacterDisplacementByRow"/> allows for tracking of how much the difference between
    /// the virtual file contents and the physical file contents when it relates to characters on a row.
    /// </summary>
    public List<(int rowIndex, int characterIndexOfInsertion, int amountOfCharactersInserted)> CharacterDisplacementByRow { get; }
    /// <summary>
    /// <see cref="CharacterDisplacementByRow"/> allows for tracking of how much the difference between
    /// the virtual file contents and the physical file contents when it relates to characters on a row.
    /// </summary>
    public List<(int insertionPoint, int amountOfRowsInserted)> RowDisplacement { get; }
}