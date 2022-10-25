using BlazorStudio.ClassLib;
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
            .AddScoped<IDefaultErrorRenderer, DefaultErrorRenderer>()
            .AddScoped<IDefaultInformationRenderer, DefaultInformationRenderer>()
            .AddBlazorStudioClassLibServices(_ => 
                new TemporaryInMemoryClipboardProvider());
    }
}