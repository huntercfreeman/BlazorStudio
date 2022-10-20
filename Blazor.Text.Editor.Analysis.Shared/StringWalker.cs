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
    private readonly string _content;
    
    public StringWalker(string content)
    {
        _content = content;
    }
    
    public int Position { get; private set; }

    public char Consume()
    {
        if (Position >= _content.Length)
            return ParserFacts.END_OF_FILE;
        
        return _content[Position++];
    }
    
    public char Peek(int offset)
    {
        if (Position + offset >= _content.Length)
            return ParserFacts.END_OF_FILE;
        
        return _content[Position + offset];
    }
    
    /// <summary>
    /// Will not allow <see cref="Position"/> to go less than 0
    /// </summary>
    public char Backtrack()
    {
        if (Position == 0)
            return ParserFacts.END_OF_FILE;
        
        Position--;

        return Peek(0);
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
        var peekBuilder = new StringBuilder();

        for (int i = 0; i < length; i++)
        {
            var currentCharacter = Peek(offset + i); 
            
            peekBuilder.Append(currentCharacter);

            if (currentCharacter == ParserFacts.END_OF_FILE)
                break;
        }

        return peekBuilder.ToString();
    }
    
    public string BacktrackRange(int length)
    {
        var backtrackBuilder = new StringBuilder();
        
        for (int i = 0; i < length; i++)
        {
            if (Position == 0)
            {
                backtrackBuilder.Append(ParserFacts.END_OF_FILE);
                return backtrackBuilder.ToString();
            }
            
            Backtrack();
            
            backtrackBuilder.Append(Peek(0));
        }

        return backtrackBuilder.ToString();
    }
    
    /// <summary>
    /// Func&lt;StringBuilder, char, int, bool&gt; predicate is to be
    /// a Func that takes the cumulative substring
    /// up to that consume iteration, and the most recently
    /// consumed character.
    /// <br/><br/>
    /// Using the cumulative substring, and
    /// most recent character - one is to return a boolean
    /// value that indicates if the while loop should terminate.
    /// <br/><br/>
    /// If the end of the string is met
    /// then the <see cref="ParserFacts.END_OF_FILE"/> character
    /// will be the final character of the returned string.
    /// </summary>
    public string DoConsumeWhile(Func<StringBuilder, char, int, bool> predicate)
    {
        var consumeBuilder = new StringBuilder();
        char mostRecentlyConsumedCharacter;
        int loopIndex = 0;
        
        do
        {
            mostRecentlyConsumedCharacter = Consume();
            
            if (mostRecentlyConsumedCharacter == ParserFacts.END_OF_FILE)
                break;
            
        } while (predicate(consumeBuilder, mostRecentlyConsumedCharacter, loopIndex++));
        
        return consumeBuilder.ToString();
    }
    
    /// <summary>
    /// Func&lt;StringBuilder, char, int, bool&gt; predicate is to be
    /// a Func that takes the cumulative substring
    /// up to that peek iteration, and the most recently
    /// peeked character.
    /// <br/><br/>
    /// Using the cumulative substring, and
    /// most recent character - one is to return a boolean
    /// value that indicates if the while loop should terminate.
    /// <br/><br/>
    /// If the end of the string is met
    /// then the <see cref="ParserFacts.END_OF_FILE"/> character
    /// will be the final character of the returned string.
    /// </summary>
    public string DoPeekWhile(Func<StringBuilder, char, int, bool> predicate, int offset)
    {
        var peekBuilder = new StringBuilder();
        char mostRecentlyPeekedCharacter;
        int peekIteration = 0;
        int loopIndex = 0;

        do
        {
            mostRecentlyPeekedCharacter = Peek(offset + peekIteration);
            
            if (mostRecentlyPeekedCharacter == ParserFacts.END_OF_FILE)
                break;
            
        } while (predicate(peekBuilder, mostRecentlyPeekedCharacter, loopIndex++));
        
        return peekBuilder.ToString();
    }
    
    /// <summary>
    /// Func&lt;StringBuilder, char, int, bool&gt; predicate is to be
    /// a Func that takes the cumulative substring
    /// up to that backtrack iteration, and the most recently
    /// backtracked to character.
    /// <br/><br/>
    /// The backtracked to character
    /// is to mean the character one would get
    /// if one invoked <see cref="Backtrack"/> and then immediately
    /// after invoked <see cref="Peek"/> with a value of 0 passed in.
    /// <br/><br/>
    /// The cumulative string made from successive invocations to <see cref="Backtrack"/>
    /// will do StringBuilder.Insert(0, mostRecentlyBacktrackedCharacter);
    /// therefore when backtracking the string is not returned in a reversed order
    /// from how it is represented in the string itself.
    /// <br/><br/>
    /// Using the cumulative substring, and
    /// most recent character - one is to return a boolean
    /// value that indicates if the while loop should terminate.
    /// <br/><br/>
    /// If the the <see cref="Position"/> would end up going out of bounds
    /// of the string then the <see cref="ParserFacts.END_OF_FILE"/> character
    /// will be the first character of the returned string.
    /// </summary>
    public string DoBacktrackWhile(Func<StringBuilder, char, int, bool> predicate)
    {
        var backtrackBuilder = new StringBuilder();
        char mostRecentlyBacktrackedCharacter;
        int loopIndex = 0;

        do
        {
            mostRecentlyBacktrackedCharacter = Backtrack();

            backtrackBuilder.Append(mostRecentlyBacktrackedCharacter);
            
            if (mostRecentlyBacktrackedCharacter == ParserFacts.END_OF_FILE)
            {
                return backtrackBuilder.ToString();
            }
            
        } while (predicate(backtrackBuilder, mostRecentlyBacktrackedCharacter, loopIndex++));
        
        return backtrackBuilder.ToString();
    }
    
    public bool CheckForSubstring(string substring)
    {
        var peekedSubstring = PeekRange(
            0, 
            substring.Length);
        
        return peekedSubstring == substring;
    }
}