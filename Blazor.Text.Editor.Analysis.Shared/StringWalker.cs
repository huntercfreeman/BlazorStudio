using System.Text;

namespace Blazor.Text.Editor.Analysis.Shared;

/// <summary>
/// The marker for the end of the string is
/// <see cref="ParserFacts.END_OF_FILE"/>.
/// <br/><br/>
/// Methods called will return <see cref="ParserFacts.END_OF_FILE"/>
/// if there was an attempt to read from beyond the final character
/// of the given string content.
/// <br/><br/>
/// If there was an attempt to read from beyond the final character
/// of the given string content then the following steps occur:
/// <br/>-<see cref="ParserFacts.END_OF_FILE"/> will be the character result or the final
/// character of a string result.
/// <br/>-The invoked method will break early even if there were more characters
/// requested than was yet read.
/// </summary>
public class StringWalker
{
    private int _position;
    private readonly string _content;
    
    public StringWalker(string content)
    {
        _content = content;
    }

    public char Consume()
    {
        if (_position >= _content.Length)
            return ParserFacts.END_OF_FILE;
        
        return _content[_position++];
    }
    
    public char Peek(int offset)
    {
        if (_position + offset >= _content.Length)
            return ParserFacts.END_OF_FILE;
        
        return _content[_position + offset];
    }
    
    /// <summary>
    /// Will not allow <see cref="_position"/> to go less than 0
    /// </summary>
    public void Backtrack()
    {
        if (_position == 0)
            return;
        
        _position--;
    }
    
    public string ConsumeRange(int length)
    {
        var consumeBuilder = new StringBuilder();

        for (int i = 0; i < length; i++)
        {
            var currentCharacter = Consume(); 
            
            consumeBuilder.Append(currentCharacter);

            if (currentCharacter == ParserFacts.END_OF_FILE)
                break;
        }

        return consumeBuilder.ToString();
    }
    
    public string PeekRange(int offset, int length)
    {
        var consumeBuilder = new StringBuilder();

        for (int i = 0; i < length; i++)
        {
            var currentCharacter = Peek(offset + i); 
            
            consumeBuilder.Append(currentCharacter);

            if (currentCharacter == ParserFacts.END_OF_FILE)
                break;
        }

        return consumeBuilder.ToString();
    }
    
    public void BacktrackRange(int length)
    {
        for (int i = 0; i < length; i++)
        {
            if (_position == 0)
                return;
            
            Backtrack();
        }
    }
}