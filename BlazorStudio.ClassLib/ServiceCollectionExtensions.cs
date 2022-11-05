using BlazorStudio.ClassLib.CommonComponents;
using BlazorStudio.ClassLib.FileSystem.Classes;
using BlazorStudio.ClassLib.FileSystem.Interfaces;
using BlazorStudio.ClassLib.Menu;
using BlazorTextEditor.RazorLib;
using BlazorTextEditor.RazorLib.Clipboard;
using Fluxor;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorStudio.ClassLib;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBlazorStudioClassLibServices(
        this IServiceCollection services,
        Func<IServiceProvider, IClipboardProvider> clipboardProviderDefaultFactory,
        ICommonComponentRenderers commonComponentRenderers)
    {
        return services
            .AddSingleton<ICommonComponentRenderers>(commonComponentRenderers)
            .AddSingleton<ICommonMenuOptionsFactory, CommonMenuOptionsFactory>()
            .AddTextEditorRazorLibServices(options =>
            {
                options.InitializeFluxor = false;
                options.ClipboardProviderFactory = clipboardProviderDefaultFactory;
            })
            .AddFluxor(options => options
                .ScanAssemblies(
                    typeof(BlazorTextEditor.RazorLib.ServiceCollectionExtensions).Assembly,
                    typeof(BlazorStudio.ClassLib.ServiceCollectionExtensions).Assembly))
            .AddScoped<IFileSystemProvider, LocalFileSystemProvider>();
    }
}