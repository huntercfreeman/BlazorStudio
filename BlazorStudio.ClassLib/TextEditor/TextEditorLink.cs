namespace BlazorStudio.ClassLib.TextEditor;

public record TextEditorLink(Guid Guid)
{
    public static TextEditorLink NewTextEditorLink()
    {
        return new(Guid.NewGuid());
    }
    
    public static TextEditorLink Empty()
    {
        return new(Guid.Empty);
    }
}