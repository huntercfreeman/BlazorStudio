using BlazorALaCarte.Shared.Clipboard;
using BlazorALaCarte.Shared.Theme;
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
            .AddScoped<ICommonComponentRenderers>(_ => commonComponentRenderers)
            .AddScoped<Menu.ICommonMenuOptionsFactory, Menu.CommonMenuOptionsFactory>()
            .AddScoped<IFileTemplateProvider, FileTemplateProvider>()
            .AddScoped<INugetPackageManagerProvider, NugetPackageManagerProviderAzureSearchUsnc>()
            .AddBlazorTextEditor(configureTextEditorServiceOptions =>
            {
                configureTextEditorServiceOptions.ClipboardProviderFactory = clipboardProviderDefaultFactory;
                configureTextEditorServiceOptions.InitialThemeRecords = BlazorStudioTextEditorColorThemeFacts.BlazorStudioTextEditorThemes;
                configureTextEditorServiceOptions.InitialThemeKey = BlazorStudioTextEditorColorThemeFacts.LightTheme.ThemeKey;
            },
            themeOptions =>
            {
                themeOptions.InitialThemeKey = ThemeFacts.VisualStudioDarkThemeClone.ThemeKey;
            })
            .AddFluxor(options => options
                .ScanAssemblies(
                    typeof(ServiceCollectionExtensions).Assembly,
                    typeof(BlazorALaCarte.Shared.ServiceCollectionExtensions).Assembly,
                    typeof(BlazorALaCarte.DialogNotification.Installation.ServiceCollectionExtensions).Assembly, 
                    typeof(BlazorALaCarte.TreeView.Installation.ServiceCollectionExtensions).Assembly,
                    typeof(BlazorTextEditor.RazorLib.ServiceCollectionExtensions).Assembly,
                    typeof(ServiceCollectionExtensions).Assembly))
            .AddScoped<IFileSystemProvider, LocalFileSystemProvider>();
    }
}