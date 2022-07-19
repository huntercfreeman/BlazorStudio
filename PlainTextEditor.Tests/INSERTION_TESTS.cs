using PlainTextEditor.ClassLib.Store.PlainTextEditorCase;

namespace PlainTextEditor.Tests;

public class INSERTION_TESTS : PLAIN_TEXT_EDITOR_STATES_TESTS
{
    private bool asyncEffectFinished;

    [Fact]
    public async Task HELLO_WORLD_C()
    {
        var plainTextEditorKey = PlainTextEditorKey.NewPlainTextEditorKey();
        Dispatcher.Dispatch(new ConstructPlainTextEditorRecordAction(plainTextEditorKey));

        string helloWorldRelativeFilePath = "./TestData/helloWorld.c";

        var expectedContent = await File
            .ReadAllTextAsync(helloWorldRelativeFilePath);

        Dispatcher.Dispatch(new PlainTextEditorInitializeAction(plainTextEditorKey,
            helloWorldRelativeFilePath));

        State.StateChanged += StateOnStateChanged;

        while (true)
        {
            if (asyncEffectFinished)
                break;

            await Task.Delay(500);
        }

        var documentPlainText = State.Value.Map[plainTextEditorKey]
            .GetPlainText();

        Assert.Equal(expectedContent, documentPlainText);

        Assert.Single(State.Value.Map);
        Assert.Single(State.Value.Array);
    }

    private void StateOnStateChanged(object? sender, EventArgs e)
    {
        asyncEffectFinished = true;
    }
}