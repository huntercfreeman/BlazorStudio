namespace BlazorStudio.ClassLib.CommonComponents;

public interface IFileFormRendererType
{
    public string FileName { get; set; }
    public bool IsDirectory { get; set; }
    public Action<string> OnAfterSubmitAction { get; set; }
}