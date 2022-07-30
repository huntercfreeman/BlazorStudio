namespace BlazorStudio.ClassLib.FileSystemApi;

public record FileHandleReadRequest(int RowIndexOffset,
    int CharacterIndexOffset, 
    int RowCount, 
    int CharacterCount,
    CancellationToken CancellationToken);