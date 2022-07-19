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
}