using BlazorStudio.ClassLib;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;

namespace BlazorStudio.RazorLib;

public static class BlazorStudioRazorLibExtensions
{
    public static IServiceCollection AddBlazorStudioRazorLibServices(this IServiceCollection services)
    {
        return services
            .AddBlazorStudioClassLibServices();
    }
}