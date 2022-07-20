using BlazorStudio.Shared.FileSystem.Interfaces;

namespace BlazorStudio.Shared.FileSystem.Classes;

/// <summary>
/// This needs to be rewritten
/// </summary>
internal class FileDescriptor : IFileDescriptor
{
    private readonly FileContentStreamer _fileContentStreamer;
    private readonly Action<IFileDescriptor> _untrackFileDescriptor;

    public FileDescriptor(IAbsoluteFilePath absoluteFilePath, int readBufferSize, Action<IFileDescriptor> untrackFileDescriptor)
    {
        AbsoluteFilePath = absoluteFilePath;
        ReadBufferSize = readBufferSize;

        _fileContentStreamer = new FileContentStreamer(absoluteFilePath, readBufferSize);
        _untrackFileDescriptor = untrackFileDescriptor;
    }

    public Guid Id { get; } = Guid.NewGuid();

    public int GetStreamPositionIndex => _fileContentStreamer.GetStreamPositionIndex;
    public IAbsoluteFilePath AbsoluteFilePath { get; }
    public int ReadBufferSize { get; }

    public char ConsumeCharacter()
    {
        return _fileContentStreamer.ConsumeCharacter();
    }

    public char PeekCharacter()
    {
        return _fileContentStreamer.PeekCharacter();
    }

    public string PeekSubstring(int length)
    {
        return _fileContentStreamer.PeekSubstring(length);
    }

    public bool MatchSubstring(string value)
    {
        return _fileContentStreamer.MatchSubstring(value);
    }

    public string ConsumeUntilPeekCharacter(char endPoint)
    {
        return _fileContentStreamer.ConsumeUntilPeekCharacter(endPoint);
    }

    public string ReadToEnd()
    {
        return _fileContentStreamer.ReadToEnd();
    }
    
    public string ReadAll()
    {
        _fileContentStreamer.ResetStreamPosition();
        return ReadToEnd();
    }

    public void ResetStreamPosition()
    {
        _fileContentStreamer.ResetStreamPosition();
    }

    public void Dispose()
    {
        _untrackFileDescriptor(this);
        _fileContentStreamer.Dispose();
    }
}