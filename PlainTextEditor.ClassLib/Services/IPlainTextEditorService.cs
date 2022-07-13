using PlainTextEditor.ClassLib.Store.PlainTextEditorCase;

namespace PlainTextEditor.ClassLib.Services;

public interface IPlainTextEditorService
{
    public Task ConstructPlainTextEditorAsync(PlainTextEditorKey plainTextEditorKey, Func<Task> plainTextEditorWasConstructedCallback);
    public void DeconstructPlainTextEditor(PlainTextEditorKey plainTextEditorKey);
}
