namespace BlazorStudio.ClassLib.Store.PlainTextEditorCase;

public interface ITextToken
{
    public TextTokenKey Key { get; }
    public string PlainText { get; }
    public string CopyText { get; }
    public TextTokenKind Kind { get; }
    public int? IndexInPlainText { get; }
}
