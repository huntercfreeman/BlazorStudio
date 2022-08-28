using BlazorStudio.ClassLib.FileSystem.Classes;
using BlazorStudio.ClassLib.FileSystem.Interfaces;
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
                .ScanAssemblies(typeof(BlazorStudioClassLibExtensions).Assembly))
            .AddScoped<IFileSystemProvider, LocalFileSystemProvider>()
            .AddScoped<INugetPackageManagerProvider, NugetPackageManagerProviderAzureSearchUsnc>();
    }
}