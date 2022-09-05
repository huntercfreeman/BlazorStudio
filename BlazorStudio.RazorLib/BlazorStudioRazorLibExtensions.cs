using BlazorStudio.ClassLib;
using BlazorStudio.ClassLib.Clipboard;
using BlazorStudio.ClassLib.Renderer;
using BlazorStudio.RazorLib.Clipboard;
using BlazorStudio.RazorLib.Notification;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorStudio.RazorLib;

public static class BlazorStudioRazorLibExtensions
{
    public static IServiceCollection AddBlazorStudioRazorLibServices(this IServiceCollection services)
    {
        return services
            .AddScoped<IClipboardProvider, TemporaryInMemoryClipboardProvider>()
            .AddScoped<IDefaultErrorRenderer, DefaultErrorRenderer>()
            .AddScoped<IDefaultInformationRenderer, DefaultInformationRenderer>()
            .AddBlazorStudioClassLibServices();
    }
}