# BlazorStudio.ClassLib/Sequence

I use [SequenceKey.cs](/BlazorStudio.ClassLib/Sequence/SequenceKey.cs) to indicate a Blazor component should rerender in
the [method override ShouldRender (web link)](https://docs.microsoft.com/en-us/aspnet/core/blazor/components/rendering)
method. That is to say if this key changes it should rerender. Otherwise it should not rerender even if the method
StateHasChanged() was called.