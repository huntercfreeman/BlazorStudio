namespace BlazorStudio.ClassLib.FileSystem.Interfaces;

public interface IFileDescriptor : IDisposable
{
    public Guid Id { get;}
    public int GetStreamPositionIndex { get; }
    public IAbsoluteFilePath AbsoluteFilePath { get; }
    public int ReadBufferSize { get; }
    
    public char ConsumeCharacter();
    public char PeekCharacter();
    public string PeekSubstring(int length);
    public bool MatchSubstring(string value);
    public string ConsumeUntilPeekCharacter(char endPoint);
    public string ReadToEnd();
    public string ReadAll();
    public void ResetStreamPosition();
}