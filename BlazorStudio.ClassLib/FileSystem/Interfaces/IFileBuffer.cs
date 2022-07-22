using System.Collections.Immutable;

namespace BlazorStudio.ClassLib.FileSystem.Interfaces;

public interface IFileBuffer
{
    public ImmutableArray<IFileDescriptor> FileDescriptors { get; }

    public event EventHandler OnFileDescriptorConstructedEventHandler;
    public event EventHandler<int> OnFileDescriptorDeconstructedEventHandler;

    public IFileDescriptor ConstructFileDescriptor(IAbsoluteFilePath absoluteFilePath, int readBufferSize = 8);
    public void DestructFileDescriptor(IFileDescriptor fileDescriptor);
}