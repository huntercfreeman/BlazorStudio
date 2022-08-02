# BlazorStudio.ClassLib/Virtualize
This directory contains all non UI C# classes necessary for the [VirtualizeCoordinateSystem.razor](/BlazorStudio.RazorLib/VirtualizeComponents/VirtualizeCoordinateSystem.razor) component.

Microsoft provides a Blazor component for vertical virtualization (see: https://docs.microsoft.com/en-us/aspnet/core/blazor/components/virtualization).

I needed to have horizontal virtualization as well.

This meant I had to write the [VirtualizeCoordinateSystem.razor](/BlazorStudio.RazorLib/VirtualizeComponents/VirtualizeCoordinateSystem.razor) component.

In short, the component has (x, y) coordinates made up of (scrollLeft, scrollTop). Using these values one can calculate the content to show.