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
        // The default constructor does not provide a preamble.
        UTF8Encoding UTF8NoPreamble = new UTF8Encoding();
        UTF8Encoding UTF8WithPreamble = new UTF8Encoding(true);

        var preambleNo = UTF8NoPreamble.GetPreamble();
        var preambleYes = UTF8WithPreamble.GetPreamble();

        /////

        string path = "./TestData/Hamlet_ Entire Play.html";

        int offset = 0;
        int length = 256;

        string encoding;
        long streamReaderStartingPosition;
        long streamReaderPositionAfterFirstPeek;
        long streamReaderEndingPosition;
        string streamReaderResult;

        using (StreamReader streamReader = new StreamReader(path, true))
        {
            streamReaderStartingPosition = streamReader.BaseStream.Position;

            _ = streamReader.Peek();

            streamReaderPositionAfterFirstPeek = streamReader.BaseStream.Position;

            var builder = new StringBuilder();

            for (int i = offset; i < length; i++)
            {
                builder.Append((char)streamReader.Read());
            }

            streamReaderEndingPosition = streamReader.BaseStream.Position;

            streamReaderResult = builder.ToString();
            encoding = streamReader.CurrentEncoding.BodyName;
        }

        using (var mmf = MemoryMappedFile.CreateFromFile(path, FileMode.Open, "blazorStudio"))
        {
            using (var accessor = mmf.CreateViewAccessor(streamReaderStartingPosition, streamReaderEndingPosition))
            {
                var z = 2;
                //int colorSize = Marshal.SizeOf<MyColor>();
                //MyColor color;

                //// Make changes to the view. 
                //for (long i = 0; i < length; i += colorSize)
                //{
                //    accessor.Read(i, out color);
                //    color.Brighten(10);
                //    accessor.Write(i, ref color);
                //}
            }
        }
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