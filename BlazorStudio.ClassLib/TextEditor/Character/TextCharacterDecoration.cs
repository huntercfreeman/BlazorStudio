namespace ExperimentalTextEditor.ClassLib.TextEditor;

public record TextCharacterDecoration
{

    // Use int for fast comparisons to see if two characters
    // have the same TextCharacterDecoration
    //
    // This allows any UserInterface using the Editor
    // to render using one span many characters that
    // share the same css styles
    //
    // I got the idea to have the characters as 'rich characters' instead
    // of Tokens from https://www.scintilla.org/ScintillaDoc.html
    // <br/>
    // I am not doing exactly what they did so if I do it wrong its my own doing.
    public byte Value { get; set; }

}