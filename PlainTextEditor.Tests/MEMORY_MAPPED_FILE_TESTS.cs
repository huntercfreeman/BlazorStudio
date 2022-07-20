using System.Diagnostics;
using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;
using System.Text;
using PlainTextEditor.ClassLib.Store.PlainTextEditorCase;
using Xunit.Abstractions;

namespace PlainTextEditor.Tests;

public class MEMORY_MAPPED_FILE_TESTS : PLAIN_TEXT_EDITOR_STATES_TESTS
{
    [Fact]
    public void HAMLET_ENTIRE_PLAY_HTML()
    {
        string path = "./TestData/Hamlet_ Entire Play.html";

        int offset = 0;
        int length = 256;

        using (StreamReader sr = new StreamReader(path, true))
        {
            var srBuilder = new StringBuilder();

            for (int i = offset; i < length; i++)
            {
                srBuilder.Append((char)sr.Read());
            }

            var srResult = srBuilder.ToString();
            var encoding = sr.CurrentEncoding.BodyName;

            var z = 2;
        }

        //using (var mmf = MemoryMappedFile.CreateFromFile(hamletEntirePlayRelativePath, FileMode.Open, "hamlet"))
        //{
        //    using (var accessor = mmf.CreateViewAccessor(0, 10))
        //    {
        //        var z = 2;
        //        //int colorSize = Marshal.SizeOf<MyColor>();
        //        //MyColor color;

        //        //// Make changes to the view. 
        //        //for (long i = 0; i < length; i += colorSize)
        //        //{
        //        //    accessor.Read(i, out color);
        //        //    color.Brighten(10);
        //        //    accessor.Write(i, ref color);
        //        //}
        //    }
        //}
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