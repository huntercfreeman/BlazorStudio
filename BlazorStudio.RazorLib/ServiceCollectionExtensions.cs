using BlazorCommon.RazorLib;
using BlazorCommon.RazorLib.Clipboard;
using BlazorCommon.RazorLib.Notification;
using BlazorCommon.RazorLib.WatchWindow.TreeViewImplementations;
using BlazorCommon.RazorLib.WatchWindow.WatchWindowExample;
using BlazorStudio.ClassLib;
using BlazorStudio.ClassLib.CommonComponents;
using BlazorStudio.ClassLib.FileSystem.Classes.Local;
using BlazorStudio.ClassLib.FileSystem.Classes.Website;
using BlazorStudio.ClassLib.FileSystem.Interfaces;
using BlazorStudio.ClassLib.Store.AccountCase;
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
        var commonRendererTypes = new CommonComponentRenderers(
            typeof(InputFileDisplay),
            typeof(CommonInformativeNotificationDisplay),
            typeof(CommonErrorNotificationDisplay),
            typeof(FileFormDisplay),
            typeof(DeleteFileFormDisplay),
            typeof(TreeViewNamespacePathDisplay),
            typeof(TreeViewExceptionDisplay),
            typeof(TreeViewAbsoluteFilePathDisplay),
            typeof(TreeViewGitFileDisplay),
            typeof(NuGetPackageManager),
            typeof(GitChangesDisplay),
            typeof(RemoveCSharpProjectFromSolutionDisplay),
            typeof(BooleanPromptOrCancelDisplay));
        
        services
            .AddSingleton<ITreeViewRenderers>(new TreeViewRenderers(
                typeof(TreeViewTextDisplay),
                typeof(TreeViewReflectionDisplay),
                typeof(TreeViewPropertiesDisplay),
                typeof(TreeViewInterfaceImplementationDisplay),
                typeof(TreeViewFieldsDisplay),
                typeof(TreeViewExceptionDisplay),
                typeof(TreeViewEnumerableDisplay)));
        
        services.AddBlazorCommonServices(options =>
        {
            options = options with
            {
                InitializeFluxor = false
            };
            
            var inBlazorCommonFactories = options.BlazorCommonFactories;

            if (isNativeApplication)
            {
                options = options with
                {
                    BlazorCommonFactories = inBlazorCommonFactories with
                    {
                        ClipboardServiceFactory = _ => new InMemoryClipboardService(true),
                    }
                };
            }

            return options;
        });
        
        services.AddBlazorTextEditor(configureTextEditorServiceOptions =>
        {
            configureTextEditorServiceOptions.InitializeFluxor = 
                false;
            
            configureTextEditorServiceOptions.InitialThemeRecords =
                BlazorStudioTextEditorColorThemeFacts.BlazorStudioTextEditorThemes;
            
            configureTextEditorServiceOptions.InitialThemeKey =
                BlazorStudioTextEditorColorThemeFacts.DarkTheme.ThemeKey;
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

        return services.AddBlazorStudioClassLibServices(
            commonRendererTypes);
    }
}