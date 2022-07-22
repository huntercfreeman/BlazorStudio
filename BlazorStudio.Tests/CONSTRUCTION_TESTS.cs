using BlazorStudio.ClassLib.Store.PlainTextEditorCase;

namespace BlazorStudio.Tests;

public class CONSTRUCTION_TESTS : PLAIN_TEXT_EDITOR_STATES_TESTS
{
    [Fact]
    public void INITIALIZE()
    {
        var plainTextEditorKey = PlainTextEditorKey.NewPlainTextEditorKey();
        Dispatcher.Dispatch(new ConstructInMemoryPlainTextEditorRecordAction(plainTextEditorKey));

        Assert.Single(State.Value.Map);
        Assert.Single(State.Value.Array);
    }
}