namespace BlazorStudio.ClassLib.FileSystemApi.MemoryMapped;

public record CharacterDisplacementValue(int RowIndex, int CharacterIndex, int InsertionAmount)
    : DisplacementValue(CharacterIndex, InsertionAmount)
{
    public override DisplacementKind DisplacementKind => DisplacementKind.Character;
}