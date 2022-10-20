namespace Blazor.Text.Editor.Analysis.CSharp.Tests;

/// <summary>
/// The <see cref="SourceCodeDecorationTests"/> are to provide as input
/// to the test a string containing the full source code of a C# snippet.
/// This snippet does not need to include invocations to other methods.
/// <br/><br/>
/// For example the method:
/// 'private void MyMethod() { Console.WriteLine("Hello World!"); }'
/// <br/><br/>
/// may be used as input as it is presumed to be a snippet of a compilable program.
/// <br/><br/>
/// For more specific tests, such as
/// the input: "Hello World!", one should refer
/// to <see cref="IndividualDecorationTests"/>
/// </summary>
public class SourceCodeDecorationTests
{
    [Fact]
    public void DecorateNone()
    {
    }
    
    [Fact]
    public void DecorateMethod()
    {
    }
    
    [Fact]
    public void DecorateType()
    {
    }
    
    [Fact]
    public void DecorateParameter()
    {
    }
    
    [Fact]
    public void DecorateStringLiteral()
    {
    }
    
    [Fact]
    public void DecorateKeyword()
    {
    }
    
    [Fact]
    public void DecorateComment()
    {
    }
}