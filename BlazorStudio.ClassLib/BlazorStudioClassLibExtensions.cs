using BlazorStudio.ClassLib.FileSystemApi;
using BlazorStudio.ClassLib.FileSystemApi.MemoryMapped;
using BlazorStudio.ClassLib.NugetPackageManager;
using Fluxor;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorStudio.ClassLib;

public static class BlazorStudioClassLibExtensions
{
    public static IServiceCollection AddBlazorStudioClassLibServices(this IServiceCollection services)
    {
        return services
            .AddFluxor(options => options
                .ScanAssemblies(
                    typeof(BlazorStudioClassLibExtensions).Assembly, 
                    typeof(PlainTextEditorClassLibExtensionMethods).Assembly
                ))
            .AddPlainTextEditorService()
            .AddScoped<IFileSystemProvider, FileSystemProvider>()
            .AddScoped<INugetPackageManagerProvider, NugetPackageManagerProviderAzureSearchUsnc>();
    }
}