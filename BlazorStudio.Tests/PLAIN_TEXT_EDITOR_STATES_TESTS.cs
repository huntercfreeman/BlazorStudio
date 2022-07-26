using BlazorStudio.ClassLib;
using BlazorStudio.ClassLib.FileSystem.Classes;
using BlazorStudio.ClassLib.FileSystem.Interfaces;
using BlazorStudio.ClassLib.Store.PlainTextEditorCase;
using Fluxor;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorStudio.Tests;

public class PLAIN_TEXT_EDITOR_STATES_TESTS
{
    protected readonly IServiceProvider ServiceProvider;
    protected readonly IDispatcher Dispatcher;
    protected readonly IStore Store;
    protected readonly IState<PlainTextEditorStates> State;
    protected readonly IAbsoluteFilePath CurrentDirectoryAbsoluteFilePath =
        new AbsoluteFilePath(Directory.GetCurrentDirectory(), true);

    // TODO: The font-size style attribute does not equal the size of the div that encapsulates the singular character. Figure out EXACTLY these values based off the font-size instead of hard coding what developer tools says
    protected double HeightOfEachRowInPixels = 27;
    protected double HidthOfEachCharacterInPixels = 9.91;

    public PLAIN_TEXT_EDITOR_STATES_TESTS()
    {
        var services = new ServiceCollection();

        services.AddPlainTextEditorClassLibServices();

        ServiceProvider = services.BuildServiceProvider();

        if (ServiceProvider is null)
        {
            throw new ApplicationException($"{nameof(ServiceProvider)} was null.");
        }

        Dispatcher = ServiceProvider.GetService<IDispatcher>()!;

        if (Dispatcher is null)
        {
            throw new ApplicationException($"{nameof(ServiceProvider)} was null.");
        }

        Store = ServiceProvider.GetRequiredService<IStore>();
        State = ServiceProvider.GetRequiredService<IState<PlainTextEditorStates>>();

        Store.InitializeAsync().Wait();
    }

    public async Task DispatchHelperAsync<T>(object action, IState<T> state, TimeSpan? pollTimeSpan = null)
    {
        if (pollTimeSpan is null)
            pollTimeSpan = TimeSpan.FromMilliseconds(500);

        bool asyncEffectFinished = false;

        void OnAsyncEventFinished(object? sender, EventArgs args) => 
            asyncEffectFinished = true;

        state.StateChanged += OnAsyncEventFinished;
        Dispatcher.Dispatch(action);

        while (true)
        {
            if (asyncEffectFinished)
                break;

            await Task.Delay(pollTimeSpan.Value);
        }

        state.StateChanged -= OnAsyncEventFinished;
    }
}