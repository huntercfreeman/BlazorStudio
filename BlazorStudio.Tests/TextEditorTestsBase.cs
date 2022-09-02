using BlazorStudio.ClassLib;
using BlazorStudio.ClassLib.FileSystem.Classes;
using BlazorStudio.ClassLib.FileSystem.Interfaces;
using BlazorStudio.ClassLib.Keyboard;
using BlazorStudio.ClassLib.Renderer;
using BlazorStudio.ClassLib.Store.TextEditorCase;
using Fluxor;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorStudio.Tests;

public class TextEditorTestsBase
{
    protected static readonly string HelloWorldInC = @"#include <stdlib.h>
#include <stdio.h>

int main() {
	printf(""Hello World!\n"");
    return 0;
}
";
    
    protected readonly IServiceProvider ServiceProvider;
    protected readonly IDispatcher Dispatcher;
    protected readonly IStore Store;
    protected readonly IFileSystemProvider FileSystemProvider;
    protected readonly IState<TextEditorStates> TextEditorStatesWrap;
    
    protected readonly IAbsoluteFilePath HelloWorldInCAbsoluteFilePath = new AbsoluteFilePath(
        "/BlazorStudioTestGround/main.c",
        false);
    
    protected readonly IAbsoluteFilePath DiscardTextFileAbsoluteFilePath = new AbsoluteFilePath(
        "/BlazorStudioTestGround/discard.txt",
        false);
    
    public TextEditorTestsBase()
    {
        var services = new ServiceCollection();

        services.AddScoped<IDefaultErrorRenderer, TestDefaultErrorRenderer>();
        
        services.AddBlazorStudioClassLibServices();

        ServiceProvider = services.BuildServiceProvider();

        FileSystemProvider = ServiceProvider.GetService<IFileSystemProvider>()!;

        // Fluxor Services
        {
            Dispatcher = ServiceProvider.GetService<IDispatcher>()!;
            Store = ServiceProvider.GetRequiredService<IStore>();

            TextEditorStatesWrap = ServiceProvider.GetRequiredService<IState<TextEditorStates>>();
            
            Store.InitializeAsync().Wait();
        }
    }

    protected async Task<List<KeyboardEventArgs>> GenerateKeyboardEventArgsFromFileAsync(IAbsoluteFilePath absoluteFilePath)
    {
        var fileContent = await FileSystemProvider.ReadFileAsync(absoluteFilePath, CancellationToken.None);

        return fileContent
            .Select(c => new KeyboardEventArgs { Key = c.ToString() })
            .ToList();
    }
}