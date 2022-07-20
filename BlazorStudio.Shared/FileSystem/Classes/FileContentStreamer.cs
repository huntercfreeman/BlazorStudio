using System.Text;
using BlazorStudio.Shared.FileSystem.Interfaces;

namespace BlazorStudio.Shared.FileSystem.Classes;

public class FileContentStreamer : IDisposable
{
    private readonly IAbsoluteFilePath _absoluteFilePath;
    private readonly char[] _buffer;

    private StreamReader _streamReader;
    private bool _bufferInitialized;
    private int _readBufferSize;
    private int _streamPositionIndex;
    private int _bufferPositionIndex;
    private int _lengthOfBufferUsed;

    public FileContentStreamer(IAbsoluteFilePath absoluteFilePath, int readBufferSize)
    {
        _absoluteFilePath = absoluteFilePath;
        _readBufferSize = readBufferSize;

        _streamReader = File.OpenText(absoluteFilePath.GetAbsoluteFilePathString());
        _buffer = new char[readBufferSize];
    }

    public int GetStreamPositionIndex => _streamPositionIndex;

    public char ConsumeCharacter()
    {
        EnsureValidBuffer();

        return _buffer[_bufferPositionIndex++];
    }

    public char PeekCharacter()
    {
        EnsureValidBuffer();

        return _buffer[_bufferPositionIndex];
    }

    public string PeekSubstring(int length)
    {
        EnsureValidBuffer();

        if(_bufferPositionIndex + length < _lengthOfBufferUsed)
        {
            var substring = new string(_buffer.AsSpan(_bufferPositionIndex, length));
            _bufferPositionIndex += length;

            return substring;
        }
        else
        {
            var maximumPossibleLength = _lengthOfBufferUsed - _bufferPositionIndex;

            if(maximumPossibleLength > 0)
            {
                var substring = new string(_buffer.AsSpan(_bufferPositionIndex, maximumPossibleLength));
                _bufferPositionIndex += maximumPossibleLength;

                return substring;
            }
            else
            {
                return "\0";
            }
        }
    }

    public bool MatchSubstring(string value)
    {
        // TODO: Don't consume if doesn't match

        EnsureValidBuffer();

        StringBuilder substringBuilder = new();

        for(int i = 0; i < value.Length; i++)
        {
            substringBuilder.Append(ConsumeCharacter());
        }

        if (value == substringBuilder.ToString())
        {
            return true;
        }
        else
        {
            // TODO: unconsume
            return false;
        }
    }

    public string ConsumeUntilPeekCharacter(char endPoint)
    {
        StringBuilder builder = new();

        char currentCharacter;

        while(((currentCharacter = PeekCharacter()) != '\0') && (currentCharacter != endPoint))
        {
            builder.Append(ConsumeCharacter());
        }

        return builder.ToString();
    }

    public string ReadToEnd()
    {
        return _streamReader.ReadToEnd();
    }
    
    public void ResetStreamPosition()
    {
        _streamReader.Dispose();
        _streamReader = File.OpenText(_absoluteFilePath.GetAbsoluteFilePathString());
    }

    private void EnsureValidBuffer()
    {
        if(_bufferPositionIndex >= _buffer.Length || !_bufferInitialized)
        {
            _bufferInitialized = true;

            _bufferPositionIndex = 0;

            _lengthOfBufferUsed = _streamReader.Read(_buffer, _bufferPositionIndex, _readBufferSize);
            _streamPositionIndex += _lengthOfBufferUsed;

            if(_lengthOfBufferUsed == 0)
            {
                for(int i = 0; i < _readBufferSize; i++)
                {
                    _buffer[i] = '\0';
                }
            }
        }
    }

    public void Dispose()
    {
        _streamReader.Dispose();
    }
}