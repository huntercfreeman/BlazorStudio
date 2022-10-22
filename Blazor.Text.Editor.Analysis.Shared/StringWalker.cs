using System.Text;
using BlazorTextEditor.RazorLib.Lexing;
using BlazorTextEditor.RazorLib.TextEditor;

namespace Blazor.Text.Editor.Analysis.Shared;

/// <summary>
/// The marker for an out of bounds read is
/// <see cref="ParserFacts.END_OF_FILE"/>.
/// <br/><br/>
/// Provides common API that can be used when implementing an <see cref="ILexer"/>
/// for the <see cref="TextEditorBase"/>.
/// <br/><br/>
/// Additionally one can write a parser that takes in a string in order to handle
/// contextual lexing. The <see cref="ILexer"/> can then traverse the parsed result
/// which might be that of a tree data structure.
/// <br/><br/>
/// I am making up the word "contextual lexing" as I am not sure the actual terminology used.
/// I am still trying to learn all the details but I mean to say, in C# var is a
/// contextual keyword. You cannot go word by word using a Lexer and determine what
/// the word 'var' entails. You instead must have a 'sentence level' understanding
/// to determine under that context whether 'var' is a keyword or if it is being used
/// as something else (perhaps a variable name?).
/// </summary>
public class StringWalker
{
    /// <summary>
    /// A private reference to the <see cref="string"/> that was provided
    /// to the <see cref="StringWalker"/>'s constructor. 
    /// </summary>
    private readonly string _content;
    
    /// <param name="content">
    /// The string that one, in a sense, wishes to step character by character through.
    /// </param>
    public StringWalker(string content)
    {
        _content = content;
    }
    
    /// <summary>
    /// The character index within the <see cref="_content"/> provided
    /// to the <see cref="StringWalker"/>'s constructor.
    /// </summary>
    public int Position { get; private set; }

    public char CurrentCharacter => Peek(0);
    public char NextCharacter => Peek(1);
    
    /// <summary>
    /// If <see cref="Position"/> is within bounds of the <see cref="_content"/>.
    /// <br/><br/>
    /// Then the character within the string <see cref="_content"/> at index
    /// of <see cref="Position"/> is returned and <see cref="Position"/> is incremented
    /// by one.
    /// <br/><br/>
    /// Otherwise, <see cref="ParserFacts.END_OF_FILE"/> is returned and
    /// the value of <see cref="Position"/> is unchanged.
    /// </summary>
    public char Consume()
    {
        if (Position >= _content.Length)
            return ParserFacts.END_OF_FILE;
        
        return _content[Position++];
    }
    
    /// <summary>
    /// If (<see cref="Position"/> + <see cref="offset"/>)
    /// is within bounds of the <see cref="_content"/>.
    /// <br/><br/>
    /// Then the character within the string <see cref="_content"/> at index
    /// of (<see cref="Position"/> + <see cref="offset"/>) is returned and
    /// <see cref="Position"/> is unchanged.
    /// <br/><br/>
    /// Otherwise, <see cref="ParserFacts.END_OF_FILE"/> is returned and
    /// the value of <see cref="Position"/> is unchanged.
    /// </summary>
    public char Peek(int offset)
    {
        if (Position + offset >= _content.Length)
            return ParserFacts.END_OF_FILE;
        
        return _content[Position + offset];
    }
    
    /// <summary>
    /// If <see cref="Position"/> being decremented by 1 would result
    /// in <see cref="Position"/> being less than 0.
    /// <br/><br/>
    /// Then <see cref="ParserFacts.END_OF_FILE"/> will be returned
    /// and <see cref="Position"/> will be left unchanged.
    /// <br/><br/>
    /// Otherwise, <see cref="Position"/> will be decremented by one
    /// and the character within the string <see cref="_content"/> at index
    /// of <see cref="Position"/> is returned.
    /// </summary>
    /// <returns>
    /// The character one would get
    /// if one invoked <see cref="Backtrack"/> and then immediately
    /// afterwards invoked <see cref="Peek"/> with a value of 0 passed in.
    /// </returns>
    public char Backtrack()
    {
        if (Position == 0)
            return ParserFacts.END_OF_FILE;
        
        Position--;

        return Peek(0);
    }
    
    /// <summary>
    /// Iterates a counter from 0 until the counter is equal to <see cref="length"/>.
    /// <br/><br/>
    /// Each iteration <see cref="Consume"/> will be invoked.
    /// <br/><br/>
    /// If an iteration's invocation of <see cref="Consume"/> returned
    /// <see cref="ParserFacts.END_OF_FILE"/> then the method will short circuit
    /// and return regardless of whether it finished iterating to <see cref="length"/>
    /// or not.
    /// </summary>
    /// <returns>
    /// The cumulative string that was built from invoking <see cref="Consume"/>
    /// <see cref="length"/> times.
    /// </returns>
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
    
    /// <summary>
    /// Iterates a counter from 0 until the counter is equal to <see cref="length"/>.
    /// <br/><br/>
    /// Each iteration <see cref="Peek"/> will be invoked using the
    /// (<see cref="offset"/> + counter).
    /// <br/><br/>
    /// If an iteration's invocation of <see cref="Peek"/> returned
    /// <see cref="ParserFacts.END_OF_FILE"/> then the method will short circuit
    /// and return regardless of whether it finished iterating to <see cref="length"/>
    /// or not.
    /// </summary>
    /// <returns>
    /// The cumulative string that was built from invoking <see cref="Peek"/>
    /// <see cref="length"/> times.
    /// </returns>
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
    
    /// <summary>
    /// Iterates a counter from 0 until the counter is equal to <see cref="length"/>.
    /// <br/><br/>
    /// Each iteration <see cref="Backtrack"/> will be invoked using the.
    /// <br/><br/>
    /// If an iteration's invocation of <see cref="Backtrack"/> returned
    /// <see cref="ParserFacts.END_OF_FILE"/> then the method will short circuit
    /// and return regardless of whether it finished iterating to <see cref="length"/>
    /// or not.
    /// </summary>
    /// <returns>
    /// The cumulative string that was built from invoking <see cref="Backtrack"/>
    /// <see cref="length"/> times.
    /// </returns>
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
    /// Form a substring of the <see cref="_content"/> that starts
    /// inclusively at the index <see cref="Position"/> and has a maximum
    /// length of <see cref="substring"/>.Length.
    /// <br/><br/>
    /// This method uses <see cref="PeekRange"/> internally and therefore
    /// will return a string that ends with <see cref="ParserFacts.END_OF_FILE"/>
    /// if an index out of bounds read was performed on <see cref="_content"/>
    /// </summary>
    public bool CheckForSubstring(string substring)
    {
        var peekedSubstring = PeekRange(
            0, 
            substring.Length);
        
        return peekedSubstring == substring;
    }
}