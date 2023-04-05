using BlazorCommon.RazorLib.BackgroundTaskCase;
using BlazorCommon.RazorLib.Clipboard;
using BlazorCommon.RazorLib.ComponentRenderers;
using BlazorCommon.RazorLib.Notification;
using BlazorCommon.RazorLib.Store.AccountCase;
using BlazorCommon.RazorLib.WatchWindow.TreeViewImplementations;
using BlazorCommon.RazorLib.WatchWindow.WatchWindowExample;
using BlazorStudio.ClassLib;
using BlazorStudio.ClassLib.ComponentRenderers;
using BlazorStudio.ClassLib.FileSystem.Classes.Local;
using BlazorStudio.ClassLib.FileSystem.Classes.Website;
using BlazorStudio.ClassLib.FileSystem.Interfaces;
using BlazorStudio.RazorLib.CSharpProjectForm;
using BlazorStudio.RazorLib.File;
using BlazorStudio.RazorLib.FormsGeneric;
using BlazorStudio.RazorLib.Git;
using BlazorStudio.RazorLib.InputFile;
using BlazorStudio.RazorLib.NuGet;
using BlazorStudio.RazorLib.TreeViewImplementations;
using BlazorTextEditor.RazorLib;
using Fluxor;
using Microsoft.Extensions.DependencyInjection;
using TreeViewExceptionDisplay = BlazorStudio.RazorLib.TreeViewImplementations.TreeViewExceptionDisplay;

namespace BlazorStudio.RazorLib;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBlazorStudioRazorLibServices(
        this IServiceCollection services,
        bool isNativeApplication)
    {
        var watchWindowTreeViewRenderers = new WatchWindowTreeViewRenderers(
            typeof(TreeViewTextDisplay),
            typeof(TreeViewReflectionDisplay),
            typeof(TreeViewPropertiesDisplay),
            typeof(TreeViewInterfaceImplementationDisplay),
            typeof(TreeViewFieldsDisplay),
            typeof(TreeViewExceptionDisplay),
            typeof(TreeViewEnumerableDisplay));
        
        var commonRendererTypes = new BlazorCommonComponentRenderers(
            typeof(BackgroundTaskDisplay),
            typeof(BooleanPromptOrCancelDisplay),
            typeof(CommonErrorNotificationDisplay),
            typeof(CommonInformativeNotificationDisplay),
            typeof(TreeViewExceptionDisplay),
            typeof(TreeViewMissingRendererFallbackDisplay),
            watchWindowTreeViewRenderers);
        
        // TODO: Move registration of "IBlazorCommonComponentRenderers" to BlazorCommon
        services.AddScoped<IBlazorCommonComponentRenderers>(_ => commonRendererTypes);
        
        var shouldInitializeFluxor = false;
        
        services.AddBlazorTextEditor(inTextEditorOptions =>
        {
            var blazorCommonOptions = 
                (inTextEditorOptions.BlazorCommonOptions ?? new()) with
            {
                InitializeFluxor = shouldInitializeFluxor
            };

            if (isNativeApplication)
            {
                var blazorCommonFactories = blazorCommonOptions.BlazorCommonFactories with
                {
                    ClipboardServiceFactory = _ => new InMemoryClipboardService(true),
                };

                blazorCommonOptions = blazorCommonOptions with
                {
                    BlazorCommonFactories = blazorCommonFactories
                };
            }
            
            return inTextEditorOptions with
            {
                InitializeFluxor = shouldInitializeFluxor,
                CustomThemeRecords = BlazorTextEditorCustomThemeFacts.AllCustomThemes,
                InitialThemeKey = BlazorTextEditorCustomThemeFacts.DarkTheme.ThemeKey,
                BlazorCommonOptions = blazorCommonOptions 
            };
        });

        Func<IServiceProvider, IEnvironmentProvider> environmentProviderFactory;
        Func<IServiceProvider, IFileSystemProvider> fileSystemProviderFactory;
        
        if (isNativeApplication)
        {
            environmentProviderFactory = _ => new LocalEnvironmentProvider();

            fileSystemProviderFactory = _ => new LocalFileSystemProvider();
        }
        else
        {
            environmentProviderFactory = serviceProvider => 
                new WebsiteEnvironmentProvider(
                    serviceProvider.GetRequiredService<IState<AccountState>>());

            fileSystemProviderFactory = serviceProvider =>
                new WebsiteFileSystemProvider(
                    serviceProvider.GetRequiredService<IEnvironmentProvider>(),
                    serviceProvider.GetRequiredService<IState<AccountState>>(),
                    serviceProvider.GetRequiredService<HttpClient>());
        }

        services
            .AddScoped<IEnvironmentProvider>(environmentProviderFactory.Invoke)
            .AddScoped<IFileSystemProvider>(fileSystemProviderFactory.Invoke);

        if (isNativeApplication)
            services.AddAuthorizationCore(); 
        
        services.AddSingleton<BlazorStudioOptions>(_ => 
            new BlazorStudioOptions(isNativeApplication));
 
        services.AddScoped<IBlazorStudioComponentRenderers>(serviceProvider =>
        {
            var blazorCommonComponentRenderers = serviceProvider
                .GetRequiredService<IBlazorCommonComponentRenderers>();

            return new BlazorStudioComponentRenderers(
                blazorCommonComponentRenderers,
                typeof(FileFormDisplay),
                typeof(DeleteFileFormDisplay),
                typeof(TreeViewNamespacePathDisplay),
                typeof(TreeViewAbsoluteFilePathDisplay),
                typeof(TreeViewGitFileDisplay),
                typeof(NuGetPackageManager),
                typeof(GitChangesDisplay),
                typeof(RemoveCSharpProjectFromSolutionDisplay),
                typeof(InputFileDisplay));
        });
        
        return services.AddBlazorStudioClassLibServices();
    }
}