namespace PlainTextEditor.ClassLib.Store.PlainTextEditorCase;

public record TextTokenKey(Guid Guid)
{
    public static TextTokenKey NewTextTokenKey()
    {
        return new TextTokenKey(Guid.NewGuid());
    }
}
