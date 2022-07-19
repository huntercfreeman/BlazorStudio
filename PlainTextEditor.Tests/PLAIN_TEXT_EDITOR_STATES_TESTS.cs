using Fluxor;
using Microsoft.Extensions.DependencyInjection;
using PlainTextEditor.ClassLib;
using PlainTextEditor.ClassLib.Store.PlainTextEditorCase;

namespace PlainTextEditor.Tests;

public class PLAIN_TEXT_EDITOR_STATES_TESTS
{
    protected readonly IServiceProvider ServiceProvider;
    protected readonly IDispatcher Dispatcher;
    protected readonly IStore Store;
    protected readonly IState<PlainTextEditorStates> State;

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