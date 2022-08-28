namespace ExperimentalTextEditor.ClassLib.TextEditor.Enums;

/// <summary>
/// <see cref="Variable"/> example: "private bool _myField;  \n public int MyProperty { get; set; }"
/// <br/><br/>
/// <see cref="Keyword"/> example: "private"
/// <br/><br/>
/// <see cref="Type"/> example: "List&lt;int&gt;"
/// <br/><br/>
/// <see cref="Method"/> example: "public void MyMethod() { ... }"
/// <br/><br/>
/// <see cref="Constant"/> example: private readonly _myReadonlyField; \n private const string STRING_CONSTANT; \n public int MyNumber { get; }
/// <br/><br/>
/// <see cref="AltFlagOne"/> see enum: <see cref="AltFlagOneDecorationKind"/>
/// <br/><br/>
/// <see cref="AltFlagTwo"/> see enum: <see cref="AltFlagTwoDecorationKind"/>
/// </summary>
[Flags]
public enum DecorationKind
{
    Variable = 1,
    Keyword = 2,
    Type = 4,
    Method = 8,
    Constant = 16,
    AltFlagOne = 32,
    AltFlagTwo = 64
}