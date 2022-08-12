using BlazorStudio.ClassLib;
using BlazorStudio.ClassLib.Clipboard;
using BlazorStudio.RazorLib.Clipboard;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorStudio.RazorLib;

public static class BlazorStudioRazorLibExtensions
{
    public static IServiceCollection AddBlazorStudioRazorLibServices(this IServiceCollection services)
    {
        return services
            .AddScoped<IClipboardProvider, TemporaryInMemoryClipboardProvider>()
            .AddBlazorStudioClassLibServices();
    }
}