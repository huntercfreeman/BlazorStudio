namespace BlazorStudio.ClassLib.FileSystemApi;

public record RowDisplacementValue(int Index, int InsertionAmount)
    : DisplacementValue(Index, InsertionAmount)
{
    public override DisplacementKind DisplacementKind => DisplacementKind.Row;
}