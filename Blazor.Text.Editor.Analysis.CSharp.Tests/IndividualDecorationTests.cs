namespace Blazor.Text.Editor.Analysis.CSharp.Tests;

/// <summary>
/// The <see cref="IndividualDecorationTests"/> are to provide as input to the test
/// a string containing the bare minimum necessary for an ILexer to correctly
/// identify a token. This string does not need to be compilable.
/// <br/><br/>
/// The test itself is to assert that the ILexer is identifying that
/// token, using the bare minimum input, correctly.
/// </summary>
public class IndividualDecorationTests
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