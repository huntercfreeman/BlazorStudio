# BlazorStudio.ClassLib Project
This C# project contains most of the C# logic for the application.

This project is a default template for C# Class Library that Microsoft provides.

This Project is rather large so I will put a README.md in every directory to talk about that directory in specific.

Though the two files that are not in a directory can be talked about here.

In [BlazorStudioClassLibExtensions.cs](/BlazorStudio.ClassLib/BlazorStudioClassLibExtensions.cs) are IServiceCollection extension methods to register necessary services. One of those services is [Fluxor (web link)](https://github.com/mrpmorris/Fluxor). Fluxor is a .NET implementation of an application architecture pattern named: [Flux (web link)](https://facebook.github.io/flux/). The final comment I'll make on this is that one might recognize the JavaScript implementation of Flux: [Redux (web link)](https://redux.js.org/)

In [PlainTextEditorClassLibExtensionMethods.cs](/BlazorStudio.ClassLib/PlainTextEditorClassLibExtensionMethods.cs) are once again IServiceCollection extension methods to register necessary services. However this file is largely a result of the 'PlainTextEditor' being an entirely separate .NET Solution I was working on. I merged the code together and have yet to fix this likely code duplication. I imagine this file can be removed by making some small changed, but I have not found the time.