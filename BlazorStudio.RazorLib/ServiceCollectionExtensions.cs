using BlazorStudio.ClassLib;
using BlazorStudio.RazorLib.Clipboard;
using BlazorStudio.RazorLib.InputFile;
using BlazorStudio.RazorLib.Notifications;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorStudio.RazorLib;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBlazorTextEditorRazorLibServices(this IServiceCollection services)
    {
        var commonRendererTypes = new CommonRendererTypes(
            typeof(InputFileDisplay),
            typeof(CommonInformativeNotificationDisplay),
            typeof(CommonErrorNotificationDisplay));
        
        return services
            .AddBlazorStudioClassLibServices(
                _ => new TemporaryInMemoryClipboardProvider(),
                commonRendererTypes);
    }
}