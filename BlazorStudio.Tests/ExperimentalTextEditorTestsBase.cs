using BlazorStudio.ClassLib;
using BlazorStudio.ClassLib.FileSystem.Classes;
using BlazorStudio.ClassLib.FileSystem.Interfaces;
using Fluxor;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorStudio.Tests;

public class ExperimentalTextEditorTestsBase
{
    protected readonly IServiceProvider ServiceProvider;
    protected readonly IDispatcher Dispatcher;
    protected readonly IStore Store;
    protected readonly IFileSystemProvider FileSystemProvider;
    
    protected readonly IAbsoluteFilePath EmptyFileAbsoluteFilePath = new AbsoluteFilePath(
        "/BlazorStudioTestGround/main.c",
        false);
    
    public ExperimentalTextEditorTestsBase()
    {
        var services = new ServiceCollection();

        services.AddBlazorStudioClassLibServices();

        ServiceProvider = services.BuildServiceProvider();

        FileSystemProvider = ServiceProvider.GetService<IFileSystemProvider>()!;

        // Fluxor Services
        {
            Dispatcher = ServiceProvider.GetService<IDispatcher>()!;
            Store = ServiceProvider.GetRequiredService<IStore>();

            Store.InitializeAsync().Wait();
        }
    }
}