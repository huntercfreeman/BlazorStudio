using BlazorStudio.ClassLib.FileSystem.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorStudio.ClassLib.FileSystem.Classes;

public static class BlazorStudioFileSystemServiceProvider
{
    public static IServiceCollection AddBlazorStudioFileSystemServices(this IServiceCollection services)
    {
        return services
            .AddFileBuffer()
            .AddFilePersister();
    }

    private static IServiceCollection AddFileBuffer(this IServiceCollection services)
    {
        return services.AddScoped<IFileBuffer, FileBuffer>();
    }
    
    private static IServiceCollection AddFilePersister(this IServiceCollection services)
    {
        return services.AddScoped<IFilePersister, FilePersister>();
    }
}