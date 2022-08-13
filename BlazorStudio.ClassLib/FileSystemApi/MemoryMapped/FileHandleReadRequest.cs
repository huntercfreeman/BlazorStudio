namespace BlazorStudio.ClassLib.FileSystemApi.MemoryMapped;

public record FileHandleReadRequest(int RowIndexOffset,
    int CharacterIndexOffset, 
    int RowCount, 
    int CharacterCount,
    CancellationToken CancellationToken);