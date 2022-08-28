namespace BlazorStudio.ClassLib.TextEditor;

public record TextEditorKey(Guid Guid)
{
    public static TextEditorKey NewTextEditorKey()
    {
        return new TextEditorKey(Guid.NewGuid());
    }
}