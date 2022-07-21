namespace BlazorStudio.ClassLib.Store.PlainTextEditorCase;

public interface IPlainTextEditorRowBuilder
{
    public IPlainTextEditorRowBuilder Add(ITextToken token);
    public IPlainTextEditorRowBuilder Insert(int index, ITextToken token);
    public IPlainTextEditorRowBuilder Remove(TextTokenKey textTokenKey);
    public IPlainTextEditorRowBuilder ReplaceMapValue(ITextToken token);
    public IPlainTextEditorRow Build();
}
