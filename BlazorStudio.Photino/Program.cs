using System;
using Microsoft.Extensions.DependencyInjection;
using Photino.Blazor;
using BlazorStudio.RazorLib;

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

            // register root component
            appBuilder.RootComponents.Add<App>("app");

            appBuilder.Services.AddBlazorStudioRazorLibServices();
            
            appBuilder.Services.AddHttpClient();

            var app = appBuilder.Build();

            // customize window
            app.MainWindow
                .SetIconFile("favicon.ico")
                .SetTitle("Photino Hello World")
                .SetDevToolsEnabled(true)
                .SetContextMenuEnabled(true)
                .SetUseOsDefaultSize(false)
                .SetSize(2500, 1750);

            AppDomain.CurrentDomain.UnhandledException += (sender, error) =>
            {
                app.MainWindow.OpenAlertWindow("Fatal exception", error.ExceptionObject.ToString());
            };

            app.Run();
        }

    }
}
