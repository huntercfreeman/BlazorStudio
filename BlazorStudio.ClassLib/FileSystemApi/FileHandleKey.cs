namespace BlazorStudio.ClassLib.FileSystemApi;

public record FileHandleKey(Guid Guid)
{
    public static FileHandleKey NewFileHandleKey()
    {
        return new FileHandleKey(Guid.NewGuid());
    }
}