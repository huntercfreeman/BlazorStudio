namespace BlazorStudio.ClassLib.TextEditor;

public record TextEditorKey(Guid Guid)
{
    public static TextEditorKey NewTextEditorKey()
    {
        return new TextEditorKey(Guid.NewGuid());
    }
    
    public static TextEditorKey Empty()
    {
        return new TextEditorKey(Guid.Empty);
    }
}