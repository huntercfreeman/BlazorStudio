using Microsoft.Extensions.DependencyInjection;
using PlainTextEditor.ClassLib;

namespace PlainTextEditor.RazorLib;

public static class PlainTextEditorRazorLibExtensionMethods
{
    public static IServiceCollection AddPlainTextEditorRazorLibServices(this IServiceCollection services)
    {
        return services
            .AddPlainTextEditorClassLibServices();
    }
}
