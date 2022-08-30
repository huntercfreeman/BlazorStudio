using System.Text;
using BlazorStudio.ClassLib.TextEditor.Enums;

namespace BlazorStudio.ClassLib.TextEditor;

/// <summary>
/// Interface exists so one can hold the <see cref="EditBlock{T}"/>
/// in a collection where each <see cref="EditBlock{T}"/> may have a varying <see cref="ValueType"/>.
/// </summary>
public interface IEditBlock
{
    /// <summary>
    /// The Type of the underlying data used to perform the edit.
    /// <br/><br/>
    /// For example, a <see cref="TextEditKind"/> of <see cref="TextEditKind.Insertion"/>
    /// would have a <see cref="ValueType"/> of <see cref="StringBuilder"/>.
    /// <br/><br/>
    /// Consecutive <see cref="TextEditKind.Insertion"/> would then see the previous <see cref="TextEditKind"/>
    /// was of the same kind. The edit would not make another <see cref="EditBlock{T}"/> but instead
    /// append to the previous insertion's StringBuilder.
    /// </summary>
    public Type ValueType { get; }
    public object UntypedValue { get; }
    public string ContentBeforeEdit { get; }
    public TextEditKind TextEditKind { get; }
    
    public string GetUserInterfaceDisplayMessage();
}