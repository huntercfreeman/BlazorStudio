using BlazorALaCarte.Shared.Clipboard;
using BlazorALaCarte.Shared.Facts;
using BlazorStudio.ClassLib.CommonComponents;
using BlazorStudio.ClassLib.FileSystem.Classes;
using BlazorStudio.ClassLib.FileSystem.Interfaces;
using BlazorStudio.ClassLib.FileTemplates;
using BlazorStudio.ClassLib.Nuget;
using BlazorTextEditor.RazorLib;
using Fluxor;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorStudio.ClassLib;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBlazorStudioClassLibServices(
        this IServiceCollection services,
        Func<IServiceProvider, IClipboardProvider> clipboardProviderDefaultFactory,
        ICommonComponentRenderers commonComponentRenderers)
    {
        return services
            .AddSingleton<ICommonComponentRenderers>(commonComponentRenderers)
            .AddSingleton<BlazorStudio.ClassLib.Menu.ICommonMenuOptionsFactory, BlazorStudio.ClassLib.Menu.CommonMenuOptionsFactory>()
            .AddSingleton<IFileTemplateProvider, FileTemplateProvider>()
            .AddSingleton<INugetPackageManagerProvider, NugetPackageManagerProviderAzureSearchUsnc>()
            .AddBlazorTextEditor(configureTextEditorServiceOptions =>
            {
                configureTextEditorServiceOptions.InitializeFluxor = false;
                configureTextEditorServiceOptions.ClipboardProviderFactory = clipboardProviderDefaultFactory;
            },
            themeOptions =>
            {
                themeOptions.InitialThemeKey = ThemeFacts.VisualStudioDarkThemeClone.ThemeKey;
            })
            .AddFluxor(options => options
                .ScanAssemblies(
                    typeof(ServiceCollectionExtensions).Assembly,
                    typeof(BlazorALaCarte.Shared.Installation.ServiceCollectionExtensions).Assembly,
                    typeof(BlazorALaCarte.DialogNotification.Installation.ServiceCollectionExtensions).Assembly, 
                    typeof(BlazorALaCarte.TreeView.Installation.ServiceCollectionExtensions).Assembly,
                    typeof(BlazorTextEditor.RazorLib.ServiceCollectionExtensions).Assembly,
                    typeof(BlazorStudio.ClassLib.ServiceCollectionExtensions).Assembly))
            .AddScoped<IFileSystemProvider, LocalFileSystemProvider>();
    }
}