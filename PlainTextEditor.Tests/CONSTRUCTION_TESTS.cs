using PlainTextEditor.ClassLib.Store.PlainTextEditorCase;

namespace PlainTextEditor.Tests;

public class CONSTRUCTION_TESTS : PLAIN_TEXT_EDITOR_STATES_TESTS
{
    [Fact]
    public void INITIALIZE()
    {
        var plainTextEditorKey = PlainTextEditorKey.NewPlainTextEditorKey();
        Dispatcher.Dispatch(new ConstructPlainTextEditorRecordAction(plainTextEditorKey));

        Assert.Single(State.Value.Map);
        Assert.Single(State.Value.Array);
    }
}