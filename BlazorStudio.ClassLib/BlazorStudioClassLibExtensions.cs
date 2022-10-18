using BlazorStudio.ClassLib.FileSystem.Classes;
using BlazorStudio.ClassLib.FileSystem.Interfaces;
using BlazorStudio.ClassLib.NugetPackageManager;
using BlazorTextEditor.RazorLib;
using Fluxor;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorStudio.ClassLib;

public static class BlazorStudioClassLibExtensions
{
    public static IServiceCollection AddBlazorStudioClassLibServices(this IServiceCollection services)
    {
        return services
            .AddTextEditorRazorLibServices(options => 
                options.InitializeFluxor = false)
            .AddFluxor(options => options
                .ScanAssemblies(
                    typeof(BlazorStudioClassLibExtensions).Assembly,
                    typeof(BlazorTextEditor.RazorLib.ServiceCollectionExtensions).Assembly))
            .AddScoped<IFileSystemProvider, LocalFileSystemProvider>()
            .AddScoped<INugetPackageManagerProvider, NugetPackageManagerProviderAzureSearchUsnc>();
    }
}