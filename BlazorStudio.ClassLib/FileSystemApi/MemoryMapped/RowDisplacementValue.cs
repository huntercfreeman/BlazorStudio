namespace BlazorStudio.ClassLib.FileSystemApi.MemoryMapped;

public record RowDisplacementValue(int Index, int ChangeAmount)
    : DisplacementValue(Index, ChangeAmount)
{
    public override DisplacementKind DisplacementKind => DisplacementKind.Row;
}