namespace BlazorStudio.ClassLib.FileSystemApi;

public record RowDisplacementValue(int Index, int ChangeAmount)
    : DisplacementValue(Index, ChangeAmount)
{
    public override DisplacementKind DisplacementKind => DisplacementKind.Row;
}