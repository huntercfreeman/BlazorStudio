# BlazorStudio.Maui Project
This C# project acts as a host for the application.

This project is a default template for Maui Blazor that Microsoft provides.

I then deleted nearly all UI related pieces (including @page "/xxxx" razor files).

In [Main.razor](/BlazorStudio.Maui/Main.razor) is the Blazor Router. I have no pages in this application. Solely there is a reference in this file to the ComponentLayout located in the BlazorStudio.RazorLib Project named, [MainLayout.razor](/BlazorStudio.RazorLib/Shared/MainLayout.razor).

```html
<!-- Main.razor -->

<Router AppAssembly="@typeof(Main).Assembly">
	<Found Context="routeData">
		<RouteView RouteData="@routeData" DefaultLayout="@typeof(BlazorStudio.RazorLib.Shared.MainLayout)" />
	</Found>
	<NotFound>
		<LayoutView Layout="@typeof(BlazorStudio.RazorLib.Shared.MainLayout)">
			<p role="alert">Sorry, there's nothing at this address.</p>
		</LayoutView>
	</NotFound>
</Router>

```

In [MauiProgram.cs](/BlazorStudio.Maui/MauiProgram.cs) I register any necessary services.

```csharp
using BlazorStudio.RazorLib;

namespace BlazorStudio.Maui
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            //  ... removed to shorten snippet ... 

            builder.Services.AddMauiBlazorWebView();

            //  ... removed to shorten snippet ... 

            builder.Services.AddBlazorStudioRazorLibServices();
            
            //  ... removed to shorten snippet ... 
        }
    }
}
```

In [wwwroot/index.html](/BlazorStudio.Maui/wwwroot/index.html) I reference the '.css' and '.js' files necessary.

```html
<!DOCTYPE html>
<html lang="en">
<head>
    <!-- ... removed to shorten snippet ... -->

    <!-- base css for BlazorStudio -->
    <link href="_content/BlazorStudio.RazorLib/blazorStudio.css" rel="stylesheet"/>
    <link href="_content/BlazorStudio.RazorLib/plainTextEditor.css" rel="stylesheet"/>

    <!-- color definition css for BlazorStudio -->
    <link href="_content/BlazorStudio.RazorLib/blazorStudio-color-variable-definitions.css" rel="stylesheet"/>

    <!-- theme css for BlazorStudio -->
    <link href="_content/BlazorStudio.RazorLib/Themes/blazorStudio-default-dark-theme.css" rel="stylesheet"/>
    <link href="_content/BlazorStudio.RazorLib/Themes/blazorStudio-default-light-theme.css" rel="stylesheet"/>
    <link href="_content/BlazorStudio.RazorLib/Themes/blazorStudio-high-contrast-default-dark-theme.css" rel="stylesheet"/>
    <link href="_content/BlazorStudio.RazorLib/Themes/blazorStudio-high-contrast-default-light-theme.css" rel="stylesheet"/>

    <!-- ... removed to shorten snippet ... -->
</head>

<body>

<!-- ... removed to shorten snippet ... -->

<script src="_framework/blazor.webview.js" autostart="false"></script>
<script src="_content/BlazorStudio.RazorLib/blazorStudio.js"></script>
<script src="_content/BlazorStudio.RazorLib/plainTextEditor.js"></script>

</body>

</html>
```

I believe that is all that goes on in this Project. I never want to say with certainty that I remembered to comment on everything, but these are the important details I remember.