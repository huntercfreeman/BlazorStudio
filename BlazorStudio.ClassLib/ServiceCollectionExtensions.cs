using BlazorStudio.ClassLib.CommonComponents;
using BlazorStudio.ClassLib.FileSystem.Classes;
using BlazorStudio.ClassLib.FileSystem.Interfaces;
using BlazorStudio.ClassLib.FileTemplates;
using BlazorStudio.ClassLib.Menu;
using BlazorTextEditor.RazorLib;
using BlazorTextEditor.RazorLib.Clipboard;
using BlazorTreeView.RazorLib;
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
            .AddSingleton<IFileTemplateProvider, FileTemplateProvider>()
            .AddTextEditorRazorLibServices(options =>
            {
                options.InitializeFluxor = false;
                options.ClipboardProviderFactory = clipboardProviderDefaultFactory;
            })
            .AddBlazorTreeViewServices(options => options.InitializeFluxor = false)
            .AddFluxor(options => options
                .ScanAssemblies(
                    typeof(BlazorTextEditor.RazorLib.ServiceCollectionExtensions).Assembly,
                    typeof(BlazorTreeView.RazorLib.ServiceCollectionExtensions).Assembly,
                    typeof(BlazorStudio.ClassLib.ServiceCollectionExtensions).Assembly))
            .AddScoped<IFileSystemProvider, LocalFileSystemProvider>();
    }
}