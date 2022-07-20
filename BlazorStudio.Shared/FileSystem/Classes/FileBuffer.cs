using System.Collections.Immutable;
using BlazorStudio.Shared.FileSystem.Interfaces;

namespace BlazorStudio.Shared.FileSystem.Classes;

internal class FileBuffer : IFileBuffer
{
    private readonly List<IFileDescriptor> _fileDescriptors = new();

    public ImmutableArray<IFileDescriptor> FileDescriptors => _fileDescriptors.ToImmutableArray();

    public event EventHandler? OnFileDescriptorConstructedEventHandler;
    public event EventHandler<int>? OnFileDescriptorDeconstructedEventHandler;

    public IFileDescriptor ConstructFileDescriptor(IAbsoluteFilePath absoluteFilePath, int readBufferSize = 8)
    {
        var fileDescriptor = new FileDescriptor(absoluteFilePath, readBufferSize, DestructFileDescriptor);

        _fileDescriptors.Add(fileDescriptor);

        OnFileDescriptorConstructedEventHandler?.Invoke(this, EventArgs.Empty);

        return fileDescriptor;
    }

    public void DestructFileDescriptor(IFileDescriptor fileDescriptor)
    {
        var indexOfRemovedFileDescriptor = _fileDescriptors.IndexOf(fileDescriptor);
        _fileDescriptors.Remove(fileDescriptor);

        OnFileDescriptorDeconstructedEventHandler?.Invoke(this, indexOfRemovedFileDescriptor);
    }
}