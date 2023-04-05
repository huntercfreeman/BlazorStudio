using System;
using System.Threading;
using BlazorCommon.RazorLib.BackgroundTaskCase;
using BlazorStudio.RazorLib;
using Microsoft.Extensions.DependencyInjection;
using Photino.Blazor;

namespace BlazorStudio.Photino
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            var appBuilder = PhotinoBlazorAppBuilder.CreateDefault(args);
            appBuilder.Services
                .AddLogging();

            appBuilder.Services.AddBlazorStudioRazorLibServices(true);

            // The code:
            //     builder.Services.AddHostedService<QueuedHostedService>();
            //
            // is not working for the Photino Blazor app.
            // So manual starting of the service is done.
            appBuilder.Services.AddSingleton<QueuedHostedService>();
            
            // register root component
            appBuilder.RootComponents.Add<App>("app");

            var app = appBuilder.Build();

            var queuedHostedService = app.Services.GetRequiredService<QueuedHostedService>();

            queuedHostedService.StartAsync(CancellationToken.None);
            
            // customize window
            app.MainWindow
                .SetIconFile("favicon.ico")
                .SetTitle("BlazorStudio")
                .SetDevToolsEnabled(true)
                .SetContextMenuEnabled(true)
                .SetUseOsDefaultSize(false)
                .SetSize(2600, 1800)
                .SetLeft(50)
                .SetTop(100);

            AppDomain.CurrentDomain.UnhandledException += (sender, error) =>
            {
                app.MainWindow.OpenAlertWindow("Fatal exception", error.ExceptionObject.ToString());
            };

            app.Run();
        }

    }
}
