using BlazorALaCarte.DialogNotification.Notification;
using BlazorStudio.ClassLib;
using BlazorStudio.ClassLib.CommonComponents;
using BlazorStudio.ClassLib.FileSystem.Classes;
using BlazorStudio.ClassLib.FileSystem.Interfaces;
using BlazorStudio.ClassLib.Store.AccountCase;
using BlazorStudio.RazorLib.Clipboard;
using BlazorStudio.RazorLib.CSharpProjectForm;
using BlazorStudio.RazorLib.File;
using BlazorStudio.RazorLib.FormsGeneric;
using BlazorStudio.RazorLib.Git;
using BlazorStudio.RazorLib.InputFile;
using BlazorStudio.RazorLib.NuGet;
using BlazorStudio.RazorLib.TreeViewImplementations;
using Fluxor;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorStudio.RazorLib;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBlazorStudioRazorLibServices(this IServiceCollection services)
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
        
        return services
            .AddBlazorStudioClassLibServices(
                _ => new TemporaryInMemoryClipboardProvider(),
                commonRendererTypes,
                serviceProvider => new WebsiteEnvironmentProvider(
                    serviceProvider.GetRequiredService<IState<AccountState>>()),
                serviceProvider => 
                    new WebsiteFileSystemProvider(
                        serviceProvider.GetRequiredService<IEnvironmentProvider>(),
                        serviceProvider.GetRequiredService<IState<AccountState>>()));
    }
}