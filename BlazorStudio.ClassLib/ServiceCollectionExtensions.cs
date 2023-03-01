using BlazorStudio.ClassLib.CommonComponents;
using BlazorStudio.ClassLib.FileTemplates;
using BlazorStudio.ClassLib.Nuget;
using Fluxor;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorStudio.ClassLib;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBlazorStudioClassLibServices(
        this IServiceCollection services,
        ICommonComponentRenderers commonComponentRenderers)
    {
        return services
            .AddScoped<ICommonComponentRenderers>(_ => commonComponentRenderers)
            .AddScoped<Menu.ICommonMenuOptionsFactory, Menu.CommonMenuOptionsFactory>()
            .AddScoped<IFileTemplateProvider, FileTemplateProvider>()
            .AddScoped<INugetPackageManagerProvider, NugetPackageManagerProviderAzureSearchUsnc>()
            .AddFluxor(options => options
                .ScanAssemblies(
                    typeof(BlazorTextEditor.RazorLib.ServiceCollectionExtensions).Assembly,
                    typeof(BlazorCommon.RazorLib.ServiceCollectionExtensions).Assembly,
                    typeof(ServiceCollectionExtensions).Assembly));
    }
}