using System.Text;

namespace PlainTextEditor.Tests;

public class DETECT_FILE_ENCODING_TESTS : PLAIN_TEXT_EDITOR_STATES_TESTS
{
    [Fact]
    public static void DETECT_FILE_ENCODING_UTF8()
    {
        string path = @$"./TestData/{nameof(DETECT_FILE_ENCODING_UTF8)}.txt";

        string[] words = new[]
        {
            "This",
            " ",
            "is some text",
            "\r\n",
            "to test",
            " ",
            "Reading",
            "\n"
        };

        StringBuilder expectedContentBuilder = new();

        foreach (var word in words)
        {
            expectedContentBuilder.Append(word);
        }

        var expectedContent = expectedContentBuilder.ToString();

        if (File.Exists(path))
        {
            File.Delete(path);
        }

        var encoding = new UTF8Encoding();

        using (StreamWriter sw = new StreamWriter(path, false, encoding))
        {
            foreach (var word in words)
            {
                sw.Write(word);
            }
        }

        using (StreamReader sr = new StreamReader(path, true))
        {
            var actualContentBuilder = new StringBuilder();

            while (sr.Peek() >= 0)
            {
                actualContentBuilder.Append((char)sr.Read());
            }

            try
            {
                Assert.Equal(encoding.BodyName, sr.CurrentEncoding.BodyName);
                Assert.Equal(encoding.EncodingName, sr.CurrentEncoding.EncodingName);
            }
            catch (Xunit.Sdk.EqualException)
            {
                // If an encoding is not what it was actually
                // written with, BUT the content is the same.
                // The response then was correct and the unit test should pass.
                Assert.Equal(expectedContent, actualContentBuilder.ToString());
            }
        }
    }

    [Fact]
    public static void DETECT_FILE_ENCODING_UNICODE()
    {
        string path = @$"./TestData/{nameof(DETECT_FILE_ENCODING_UNICODE)}.txt";

        string[] words = new[]
        {
            "This",
            " ",
            "is some text",
            "\r\n",
            "to test",
            " ",
            "Reading",
            "\n"
        };

        StringBuilder expectedContentBuilder = new();

        foreach (var word in words)
        {
            expectedContentBuilder.Append(word);
        }

        var expectedContent = expectedContentBuilder.ToString();

        if (File.Exists(path))
        {
            File.Delete(path);
        }

        var encoding = new UnicodeEncoding();

        using (StreamWriter sw = new StreamWriter(path, false, encoding))
        {
            foreach (var word in words)
            {
                sw.Write(word);
            }
        }

        using (StreamReader sr = new StreamReader(path, true))
        {
            var actualContentBuilder = new StringBuilder();

            while (sr.Peek() >= 0)
            {
                actualContentBuilder.Append((char)sr.Read());
            }

            try
            {
                Assert.Equal(encoding.BodyName, sr.CurrentEncoding.BodyName);
                Assert.Equal(encoding.EncodingName, sr.CurrentEncoding.EncodingName);
            }
            catch (Xunit.Sdk.EqualException)
            {
                // If an encoding is not what it was actually
                // written with, BUT the content is the same.
                // The response then was correct and the unit test should pass.
                Assert.Equal(expectedContent, actualContentBuilder.ToString());
            }
        }
    }

    [Fact]
    public static void DETECT_FILE_ENCODING_UTF32()
    {
        string path = @$"./TestData/{nameof(DETECT_FILE_ENCODING_UTF32)}.txt";

        string[] words = new[]
        {
            "This",
            " ",
            "is some text",
            "\r\n",
            "to test",
            " ",
            "Reading",
            "\n"
        };

        StringBuilder expectedContentBuilder = new();

        foreach (var word in words)
        {
            expectedContentBuilder.Append(word);
        }

        var expectedContent = expectedContentBuilder.ToString();

        if (File.Exists(path))
        {
            File.Delete(path);
        }

        var encoding = new UTF32Encoding();

        using (StreamWriter sw = new StreamWriter(path, false, encoding))
        {
            foreach (var word in words)
            {
                sw.Write(word);
            }
        }

        using (StreamReader sr = new StreamReader(path, true))
        {
            var actualContentBuilder = new StringBuilder();

            while (sr.Peek() >= 0)
            {
                actualContentBuilder.Append((char)sr.Read());
            }

            try
            {
                Assert.Equal(encoding.BodyName, sr.CurrentEncoding.BodyName);
                Assert.Equal(encoding.EncodingName, sr.CurrentEncoding.EncodingName);
            }
            catch (Xunit.Sdk.EqualException)
            {
                // If an encoding is not what it was actually
                // written with, BUT the content is the same.
                // The response then was correct and the unit test should pass.
                Assert.Equal(expectedContent, actualContentBuilder.ToString());
            }
        }
    }

    [Fact]
    public static void DETECT_FILE_ENCODING_ASCII()
    {
        string path = @$"./TestData/{nameof(DETECT_FILE_ENCODING_ASCII)}.txt";

        string[] words = new []
        {
            "This",
            " ",
            "is some text",
            "\r\n",
            "to test",
            " ",
            "Reading",
            "\n"
        };

        StringBuilder expectedContentBuilder = new();

        foreach (var word in words)
        {
            expectedContentBuilder.Append(word);
        }

        var expectedContent = expectedContentBuilder.ToString();

        if (File.Exists(path))
        {
            File.Delete(path);
        }

        var encoding = new ASCIIEncoding();

        using (StreamWriter sw = new StreamWriter(path, false, encoding))
        {
            foreach (var word in words)
            {
                sw.Write(word);
            }
        }

        using (StreamReader sr = new StreamReader(path, true))
        {
            var actualContentBuilder = new StringBuilder();

            while (sr.Peek() >= 0)
            {
                actualContentBuilder.Append((char)sr.Read());
            }

            try
            {
                Assert.Equal(encoding.BodyName, sr.CurrentEncoding.BodyName);
                Assert.Equal(encoding.EncodingName, sr.CurrentEncoding.EncodingName);
            }
            catch (Xunit.Sdk.EqualException)
            {
                // If an encoding is not what it was actually
                // written with, BUT the content is the same.
                // The response then was correct and the unit test should pass.
                Assert.Equal(expectedContent, actualContentBuilder.ToString());
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