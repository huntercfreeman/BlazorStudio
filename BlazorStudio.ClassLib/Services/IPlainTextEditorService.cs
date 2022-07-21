using BlazorStudio.ClassLib.Store.PlainTextEditorCase;

namespace BlazorStudio.ClassLib.Services;

public interface IPlainTextEditorService
{
    public Task ConstructPlainTextEditorAsync(PlainTextEditorKey plainTextEditorKey, Func<Task> plainTextEditorWasConstructedCallback);
    public void DeconstructPlainTextEditor(PlainTextEditorKey plainTextEditorKey);
}
