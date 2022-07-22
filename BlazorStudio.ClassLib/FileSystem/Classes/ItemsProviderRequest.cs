using BlazorStudio.ClassLib.Virtualize;

namespace BlazorStudio.ClassLib.FileSystem.Classes;

public readonly struct FileCoordinateGridRequest
{
    public int StartingRowIndex { get; }
    public int RowCount { get; }
    public CancellationToken CancellationToken { get; }

    public FileCoordinateGridRequest(int startingRowIndex, int rowCount, CancellationToken cancellationToken)
    {
        StartingRowIndex = startingRowIndex;
        RowCount = rowCount;
        CancellationToken = cancellationToken;
    }
}