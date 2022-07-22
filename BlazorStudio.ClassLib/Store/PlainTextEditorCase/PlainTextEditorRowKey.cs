namespace BlazorStudio.ClassLib.Store.PlainTextEditorCase;

public record PlainTextEditorRowKey(Guid Guid)
{
    public static PlainTextEditorRowKey NewPlainTextEditorRowKey()
    {
        return new PlainTextEditorRowKey(Guid.NewGuid());
    }
}
