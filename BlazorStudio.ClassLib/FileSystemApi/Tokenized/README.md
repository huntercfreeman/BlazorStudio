# BlazorStudio.ClassLib/FileSystemApi/Tokenized

The first way I wrote the [PlainTextEditor](/BlazorStudio.ClassLib/Store/PlainTextEditorCase/) PlainTextEditor was with a  [MemoryMappedFile](https://docs.microsoft.com/en-us/dotnet/standard/io/memory-mapped-files).

In short I did the [MemoryMappedFile](https://docs.microsoft.com/en-us/dotnet/standard/io/memory-mapped-files) version because I thought it best not to read in the entirety of a file's contents as the file might be very large.

I will keep the [../MemoryMapped/](/BlazorStudio.ClassLib/FileSystemApi/MemoryMapped/) directory as I still believe it useful. For example, in Visual Studio Code when I open a JSON document sometimes I see this message

> Tokenization is skipped for long lines for performance reasons. This can be configured via editor.maxTokenizationLineLength.

It appears there are cases where I need a tokenized file, and at time where I need a non-tokenized version. So as previously stated I will keep around the [../MemoryMapped/](/BlazorStudio.ClassLib/FileSystemApi/MemoryMapped/) directory as it might be useful in the future.