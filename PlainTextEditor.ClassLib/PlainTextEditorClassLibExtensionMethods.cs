using Fluxor;
using Microsoft.Extensions.DependencyInjection;
using PlainTextEditor.ClassLib.Services;

namespace PlainTextEditor.ClassLib;

public static class PlainTextEditorClassLibExtensionMethods
{
    public static IServiceCollection AddPlainTextEditorClassLibServices(this IServiceCollection services)
    {
        return services
            .AddFluxor(options => options
                .ScanAssemblies(typeof(PlainTextEditorClassLibExtensionMethods).Assembly))
            .AddPlainTextEditorService();
    }
    
    private static IServiceCollection AddPlainTextEditorService(this IServiceCollection services)
    {
        return services.AddScoped<IPlainTextEditorService, PlainTextEditorService>();
    }
}
