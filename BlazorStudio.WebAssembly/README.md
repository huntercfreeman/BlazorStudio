# BlazorStudio.WebAssembly Project

This C# project acts as a host for the application.

This project is a default template for Blazor WebAssembly that Microsoft provides.

I then deleted nearly all UI related pieces (including @page "/xxxx" razor files).

In [App.razor](/BlazorStudio.WebAssembly/App.razor) is the Blazor Router. I have no pages in this application. Solely
there is a reference in this file to the ComponentLayout located in the BlazorStudio.RazorLib Project
named, [MainLayout.razor](/BlazorStudio.RazorLib/Shared/MainLayout.razor).

```html
<!-- App.razor -->

<Router AppAssembly="@typeof(Program).Assembly">
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

In [Program.cs](/BlazorStudio.WebAssembly/Program.cs) I register any necessary services.

```csharp
//  ... removed to shorten snippet ... 

builder.Services.AddBlazorStudioRazorLibServices();

//  ... removed to shorten snippet ... 
```

In [wwwroot/index.html](/BlazorStudio.WebAssembly/wwwroot/index.html) I reference the '.css' and '.js' files necessary.

```html
<!DOCTYPE html>
<html lang="en">

<head>
    <!-- ... removed to shorten snippet ... -->

    <!-- base css for BlazorStudio -->
    <link href="_content/BlazorStudio.RazorLib/blazorStudio.css" rel="stylesheet" />
    <link href="_content/BlazorStudio.RazorLib/plainTextEditor.css" rel="stylesheet" />

    <!-- color definition css for BlazorStudio -->
    <link href="_content/BlazorStudio.RazorLib/blazorStudio-color-variable-definitions.css" rel="stylesheet" />

    <!-- theme css for BlazorStudio -->
    <link href="_content/BlazorStudio.RazorLib/Themes/blazorStudio-default-dark-theme.css" rel="stylesheet" />
    <link href="_content/BlazorStudio.RazorLib/Themes/blazorStudio-default-light-theme.css" rel="stylesheet" />
    <link href="_content/BlazorStudio.RazorLib/Themes/blazorStudio-high-contrast-default-dark-theme.css" rel="stylesheet" />
    <link href="_content/BlazorStudio.RazorLib/Themes/blazorStudio-high-contrast-default-light-theme.css" rel="stylesheet" />

    <!-- ... removed to shorten snippet ... -->
</head>

<body>
    <!-- ... removed to shorten snippet ... -->
    
    <script src="_framework/blazor.webassembly.js"></script>
    <script>navigator.serviceWorker.register('service-worker.js');</script>

    <script src="_content/BlazorStudio.RazorLib/blazorStudio.js"></script>
    <script src="_content/BlazorStudio.RazorLib/plainTextEditor.js"></script>
</body>

</html>
```

I believe that is all that goes on in this Project. I never want to say with certainty that I remembered to comment on
everything, but these are the important details I remember.