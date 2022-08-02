# BlazorStudio.ClassLib/FileSystemApi
This directory is related to the PlainTextEditor. Below I copy and pasted the [README.md for the PlainTextEditor](/BlazorStudio.ClassLib/Store/PlainTextEditorCase/README.md).

The PlainTextEditor is asynchronous, and immutable.

The PlainTextEditor works in the following way:

An editor gets a reference to an [IFileHandle.cs](/BlazorStudio.ClassLib/FileSystemApi/IFileHandle.cs). IFileHandle is an abstraction over a [MemoryMappedFile](https://docs.microsoft.com/en-us/dotnet/standard/io/memory-mapped-files) that allows editing of a virtual file which is mapped to a physical file.

The [PlainTextEditorDisplay.razor](/BlazorStudio.RazorLib/PlainTextEditorCase/PlainTextEditorDisplay.razor) component leverages the [VirtualizeCoordinateSystem.razor](/BlazorStudio.RazorLib/VirtualizeComponents/VirtualizeCoordinateSystem.razor) component. This allows for virtualization both vertically, and horizontally.

The (scrollLeft, scrollTop) coordinates provided by the virtualization are used in combination with the viewport to calculate what is visible by the user (this is a short and incomplete description).

A request then is made to the [IFileHandle.cs](/BlazorStudio.ClassLib/FileSystemApi/IFileHandle.cs). The IFileHandle then relays that request to the underlying [MemoryMappedFile](https://docs.microsoft.com/en-us/dotnet/standard/io/memory-mapped-files). After the MemoryMappedFile returns the content in the physical file a List of pending edits (see: [EditBuilder.Main.cs](/BlazorStudio.ClassLib/FileSystemApi/EditBuilder.Main.cs)) are iterated and performed to what was returned. This results in the virtual file which is then returned and displayed on the UI.

The PlainTextEditor maintains mainly a List of edits to a physical file. As well the PlainTextEditor is likely not to be a complete representation of the physical file. Instead in most cases it is a part of the much larger physical file being read into memory.