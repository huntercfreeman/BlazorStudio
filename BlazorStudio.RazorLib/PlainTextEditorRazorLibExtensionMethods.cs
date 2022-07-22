using BlazorStudio.ClassLib;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorStudio.RazorLib;

public static class PlainTextEditorRazorLibExtensionMethods
{
    public static IServiceCollection AddPlainTextEditorRazorLibServices(this IServiceCollection services)
    {
        return services
            .AddPlainTextEditorClassLibServices();
    }
}
