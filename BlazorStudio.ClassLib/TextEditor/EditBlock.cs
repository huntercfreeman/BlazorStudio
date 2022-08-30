using BlazorStudio.ClassLib.TextEditor.Enums;

namespace BlazorStudio.ClassLib.TextEditor;

public class EditBlock<T> : IEditBlock
{
    public EditBlock(TextEditKind textEditKind, T value, string contentBeforeEdit)
    {
        TextEditKind = textEditKind;
        TypedValue = value;
        ContentBeforeEdit = contentBeforeEdit;
    }

    public Type ValueType => typeof(T);
    public object UntypedValue => TypedValue;
    public string ContentBeforeEdit { get; }
    public TextEditKind TextEditKind { get; }
    public T TypedValue { get; }

    public string GetUserInterfaceDisplayMessage()
    {
        return $"{TextEditKind} {TypedValue}";
    }
}