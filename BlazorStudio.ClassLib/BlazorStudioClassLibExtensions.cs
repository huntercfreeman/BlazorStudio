using BlazorStudio.ClassLib.FileSystem.Classes;
using BlazorStudio.ClassLib.FileSystem.Interfaces;
using BlazorStudio.ClassLib.NugetPackageManager;
using BlazorTextEditor.RazorLib;
using BlazorTextEditor.RazorLib.Clipboard;
using Fluxor;
using Microsoft.Extensions.DependencyInjection;
using ServiceCollectionExtensions = BlazorTextEditor.RazorLib.ServiceCollectionExtensions;

namespace BlazorStudio.ClassLib;

public static class BlazorStudioClassLibExtensions
{
    public static IServiceCollection AddBlazorStudioClassLibServices(
        this IServiceCollection services,
        Func<IServiceProvider, IClipboardProvider> clipboardProviderDefaultFactory)
    {
        return services
            .AddTextEditorRazorLibServices(options =>
            {
                options.InitializeFluxor = false;
                options.ClipboardProviderFactory = clipboardProviderDefaultFactory;
            })
            .AddFluxor(options => options
                .ScanAssemblies(
                    typeof(BlazorStudioClassLibExtensions).Assembly,
                    typeof(ServiceCollectionExtensions).Assembly))
            .AddScoped<IFileSystemProvider, LocalFileSystemProvider>()
            .AddScoped<INugetPackageManagerProvider, NugetPackageManagerProviderAzureSearchUsnc>();
    }
}