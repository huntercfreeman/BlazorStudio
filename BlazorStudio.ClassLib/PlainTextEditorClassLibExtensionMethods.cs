using BlazorStudio.ClassLib.Services;
using Fluxor;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorStudio.ClassLib;

public static class PlainTextEditorClassLibExtensionMethods
{
    public static IServiceCollection AddPlainTextEditorClassLibServices(this IServiceCollection services)
    {
        return services
            .AddFluxor(options => options
                .ScanAssemblies(typeof(PlainTextEditorClassLibExtensionMethods).Assembly))
            .AddPlainTextEditorService();
    }
    
    public static IServiceCollection AddPlainTextEditorService(this IServiceCollection services)
    {
        return services.AddScoped<IPlainTextEditorService, PlainTextEditorService>();
    }
}
