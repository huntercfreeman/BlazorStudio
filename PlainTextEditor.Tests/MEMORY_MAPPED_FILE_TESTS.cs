using System.Diagnostics;
using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;
using System.Text;
using PlainTextEditor.ClassLib.Store.PlainTextEditorCase;
using Xunit.Abstractions;

namespace PlainTextEditor.Tests;

public class MEMORY_MAPPED_FILE_TESTS : PLAIN_TEXT_EDITOR_STATES_TESTS
{
    /// <summary>
    /// Goal with this Test is NOT to look at using <see cref="MemoryMappedFile"/> Virtualization but instead
    /// read the entire file and compare it against <see cref="StreamReader"/> so it can be asserted that the binary
    /// is being transcoded properly.
    /// </summary>
    [Fact]
    public void MEMORY_MAPPED_FILE_IS_EQUAL_TO_STREAM_READER()
    {
        string path = "./TestData/Hamlet_ Entire Play.html";

        Encoding encoding;
        long streamReaderEndingPosition;
        string streamReaderResult;

        using (StreamReader streamReader = new StreamReader(path, true))
        {
            var builder = new StringBuilder();

            while (streamReader.Peek() != -1)
            {
                builder.Append((char)streamReader.Read());
            }

            streamReaderEndingPosition = streamReader.BaseStream.Position;

            streamReaderResult = builder.ToString();
            encoding = streamReader.CurrentEncoding;
        }

        // TODO: startOfText is seemingly not needed it appears MemoryMappedFile.CreateFromFile will skip the BOM for us
        // var startOfText = streamReaderStartingPosition + encoding.Preamble.Length;
        var contentLength = streamReaderEndingPosition;

        var buffer = new byte[contentLength];

        using (var mmf = MemoryMappedFile.CreateFromFile(path, FileMode.Open, "blazorStudio"))
        {
            using (var accessor = mmf.CreateViewAccessor(0, streamReaderEndingPosition))
            {
                if (contentLength > Int32.MaxValue)
                {
                    throw new ApplicationException(
                        $"Requested {nameof(contentLength)}: '{contentLength}' is larger than {nameof(Int32)} {nameof(Int32.MaxValue)}.");
                }

                accessor.ReadArray(0, buffer, 0, (int) contentLength);
            }
        }

        string memoryMappedFileResult;

        using (StreamReader streamReader = new StreamReader(new MemoryStream(buffer), encoding))
        {
            var builder = new StringBuilder();

            while (streamReader.Peek() != -1)
            {
                builder.Append((char)streamReader.Read());
            }

            memoryMappedFileResult = builder.ToString();
        }

        Assert.Equal(streamReaderResult, memoryMappedFileResult);
    }

    /// <summary>
    /// Goal with this Test is to look at using
    /// <see cref="MemoryMappedFile"/> Virtualization by random access memory
    /// </summary>
    [Theory]
    [InlineData("./TestData/Hamlet_ Entire Play.html", 0, 1024)]
    [InlineData("./TestData/Hamlet_ Entire Play.html", 1024, 1024)]
    public void RANDOM_ACCESS_MEMORY_MAPPED_FILE_IS_EQUAL_TO_STREAM_READER(string path,
        long startingPosition,
        int readLength)
    {
        Encoding encoding;
        string streamReaderResult;

        using (StreamReader streamReader = new StreamReader(path, true))
        {
            if (!streamReader.BaseStream.CanSeek)
            {
                throw new ApplicationException(
                    $"{nameof(streamReader.BaseStream.CanSeek)} was {streamReader.BaseStream.CanSeek}.");
            }
            
            streamReader.BaseStream.Seek(startingPosition, SeekOrigin.Begin);

            var builder = new StringBuilder();

            var i = 0;

            while (i++ < readLength && streamReader.Peek() != -1)
            {
                builder.Append((char)streamReader.Read());
            }

            streamReaderResult = builder.ToString();
            encoding = streamReader.CurrentEncoding;
        }

        var buffer = new byte[readLength];

        using (var mmf = MemoryMappedFile.CreateFromFile(path, FileMode.Open, "blazorStudio"))
        {
            using (var accessor = mmf.CreateViewAccessor(startingPosition, readLength))
            {
                accessor.ReadArray(0, buffer, 0, readLength);
            }
        }

        string memoryMappedFileResult;

        using (StreamReader streamReader = new StreamReader(new MemoryStream(buffer), encoding))
        {
            var builder = new StringBuilder();

            var i = 0;

            while (i++ < readLength && streamReader.Peek() != -1)
            {
                builder.Append((char)streamReader.Read());
            }

            memoryMappedFileResult = builder.ToString();
        }

        Assert.Equal(streamReaderResult, memoryMappedFileResult);
    }
}