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
    /// Goal with this Test is not to look at using <see cref="MemoryMappedFile"/> Virtualization but instead
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
    
    ////https://stackoverflow.com/questions/30251443/how-to-read-and-write-a-file-using-memory-mapped-file-c
    //[Fact]
    //public void MEMORY_MAPPED_FILE_USAGE()
    //{
    //    long offset = 0x10000000; // 256 megabytes 
    //    long length = 0x20000000; // 512 megabytes 

    //    // Create the memory-mapped file. 
    //    using (var mmf = MemoryMappedFile.CreateFromFile(@"c:\ExtremelyLargeImage.data", FileMode.Open, "ImgA"))
    //    {
    //        // Create a random access view, from the 256th megabyte (the offset) 
    //        // to the 768th megabyte (the offset plus length). 
    //        using (var accessor = mmf.CreateViewAccessor(offset, length))
    //        {
    //            int colorSize = Marshal.SizeOf<MyColor>();
    //            MyColor color;

    //            // Make changes to the view. 
    //            for (long i = 0; i < length; i += colorSize)
    //            {
    //                accessor.Read(i, out color);
    //                color.Brighten(10);
    //                accessor.Write(i, ref color);
    //            }
    //        }
    //    }
    //}
}