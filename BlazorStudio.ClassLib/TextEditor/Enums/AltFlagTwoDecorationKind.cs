namespace ExperimentalTextEditor.ClassLib.TextEditor.Enums;

/// <summary>
/// <see cref="Warning"/> example: "variable is unused"
/// <br/><br/>
/// <see cref="Error"/> example: "variable is not found"
/// <br/><br/>
/// <see cref="Info"/> example: "explicit type declaration when code style settings says use var"
/// <br/><br/>
/// <see cref="Comment"/> example: "// Register services here"
/// <br/><br/>
/// <see cref="Summary"/> this is a summary so this one is awkward to type 
/// <br/><br/>
/// <see cref="Whitespace"/> example: " "
/// <br/><br/>
/// <see cref="RuntimeWarning"/> example: "hot reload cannot capture a closure variable added while running the app, must restart the app"
/// </summary>
[Flags]
public enum AltFlagTwoDecorationKind
{
    Warning = 1,
    Error = 2,
    Info = 4,
    Comment = 8,
    Summary = 16,
    Whitespace = 32,
    RuntimeWarning = 64
}