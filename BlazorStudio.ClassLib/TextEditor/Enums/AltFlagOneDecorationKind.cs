namespace ExperimentalTextEditor.ClassLib.TextEditor.Enums;

/// <summary>
/// <see cref="Local"/> example: "private void MyMethod() { var myLocalVariable = 45; }"
/// <br/><br/>
/// <see cref="KeywordOther"/> example: "@code{ MyComponent.razor logic }"
/// <br/><br/>
/// <see cref="KeywordControl"/> example: "for"
/// <br/><br/>
/// <see cref="NonStringValue"/> example: "var nonStringValue = 1;"
/// <br/><br/>
/// <see cref="StringValue"/> example: "var stringValue = 'This is a string';"
/// <br/><br/>
/// <see cref="Tag"/> example: "&lt;div&gt;"
/// <br/><br/>
/// <see cref="TagAttributeName"/> example: "&lt;div style='...'&gt;"
/// </summary>
[Flags]
public enum AltFlagOneDecorationKind
{
    Local = 1,
    KeywordOther = 2,
    KeywordControl = 4,
    NonStringValue = 8,
    StringValue = 16,
    Tag = 32,
    TagAttributeName = 64
}