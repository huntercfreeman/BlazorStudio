namespace BlazorStudio.ClassLib.FileSystem.Classes;

public readonly struct FileCoordinateGridRequest
{
    public int StartingRowIndex { get; }
    public int RowCount { get; }
    
    public int StartingCharacterIndex { get; }
    public int CharacterCount { get; }

    public CancellationToken CancellationToken { get; }

    public FileCoordinateGridRequest(int startingRowIndex, int rowCount, int startingCharacterIndex, int characterCount, CancellationToken cancellationToken)
    {
        StartingRowIndex = startingRowIndex;
        RowCount = rowCount;
        StartingCharacterIndex = startingCharacterIndex;
        CharacterCount = characterCount;
        CancellationToken = cancellationToken;
    }
}