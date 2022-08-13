namespace BlazorStudio.ClassLib.FileSystemApi.MemoryMapped;

public record FileHandleKey(Guid Guid)
{
    public static FileHandleKey NewFileHandleKey()
    {
        return new FileHandleKey(Guid.NewGuid());
    }
}