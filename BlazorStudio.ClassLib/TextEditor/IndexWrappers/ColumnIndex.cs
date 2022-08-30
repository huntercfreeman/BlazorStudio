namespace BlazorStudio.ClassLib.TextEditor.IndexWrappers;

public class ColumnIndex
{
    public ColumnIndex(int value)
    {
        Value = value;
    }
    
    public ColumnIndex(ColumnIndex columnIndex)
    {
        Value = columnIndex.Value;
    }
    
    public int Value { get; set; }
}