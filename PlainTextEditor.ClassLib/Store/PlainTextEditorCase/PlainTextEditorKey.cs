namespace PlainTextEditor.ClassLib.Store.PlainTextEditorCase;

public record PlainTextEditorKey(Guid Guid)
{
    public static PlainTextEditorKey NewPlainTextEditorKey()
    {
        return new PlainTextEditorKey(Guid.NewGuid());
    }
}
