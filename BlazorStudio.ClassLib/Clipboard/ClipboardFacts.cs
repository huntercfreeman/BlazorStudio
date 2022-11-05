namespace BlazorStudio.ClassLib.Clipboard;

/// <summary>
/// I am not quite sure how to interact with clipboard when it comes to
/// transferring objects.
/// <br/><br/>
/// For now I will use this identifier but I need to look into what other
/// people are doing for this.
/// <br/><br/>
/// This is used for example when copying the selected TreeViewEntry in the
/// solution explorer so it can be pasted later on somewhere else.
/// </summary>
public static class ClipboardFacts
{
    /// <summary>
    /// Indicates the start of a phrase.
    /// <br/><br/>
    /// Phrase is being defined as a tag, command, datatype and value
    /// in string form.
    /// <br/><br/>
    /// The string unescaped: `'";bstudio
    /// </summary>
    public const string Tag = "`'\";bstudio";

    /// <summary>
    /// Deliminates tag_command_datatype_value
    /// </summary>
    public const string FieldDelimiter = "_";
    
    // Commands
    public const string CopyCommand = "copy";
    public const string CutCommand = "cut";
    
    // DataTypes
    public const string AbsoluteFilePathDataType = "absolute-file-path";

    public static string FormatPhrase(
        string command, 
        string dataType, 
        string value)
    {
        return Tag +
               FieldDelimiter +
               command +
               FieldDelimiter +
               dataType +
               FieldDelimiter +
               value;
    }

    public static bool TryParseString(
        string clipboardContents, 
        out ClipboardPhrase? clipboardPhrase)
    {
        clipboardPhrase = null;
        
        if (clipboardContents.StartsWith(ClipboardFacts.Tag))
        {
            // Skip Tag
            clipboardContents = clipboardContents
                .Substring(ClipboardFacts.Tag.Length);
            // Skip Delimiter following the Tag
            clipboardContents = clipboardContents
                .Substring(ClipboardFacts.FieldDelimiter.Length);

            var nextDelimiter = clipboardContents.IndexOf(
                ClipboardFacts.FieldDelimiter, 
                StringComparison.Ordinal);
            
            // Take Command
            var command = clipboardContents
                .Substring(0, nextDelimiter);
            
            clipboardContents = clipboardContents
                .Substring(nextDelimiter + 1);
            
            nextDelimiter = clipboardContents.IndexOf(
                ClipboardFacts.FieldDelimiter, 
                StringComparison.Ordinal);
            
            // Take DataType
            var dataType = clipboardContents
                .Substring(0, nextDelimiter);

            // Value is whatever remains in the string
            var value = clipboardContents
                .Substring(nextDelimiter + 1);

            clipboardPhrase = new ClipboardPhrase
            {
                Command = command,
                DataType = dataType,
                Value = value
            };

            return true;
        }

        return false;
    }
}