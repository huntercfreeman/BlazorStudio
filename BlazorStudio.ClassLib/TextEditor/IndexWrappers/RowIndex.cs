namespace BlazorStudio.ClassLib.TextEditor.IndexWrappers;

public class RowIndex
{
    public RowIndex(int value)
    {
        Value = value;
    }
    
    public RowIndex(RowIndex rowIndex)
    {
        Value = rowIndex.Value;
    }
    
    public int Value { get; set; }
}