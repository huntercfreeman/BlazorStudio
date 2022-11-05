using System.Collections.Immutable;
using BlazorStudio.ClassLib.FileTemplates;

namespace BlazorStudio.ClassLib.CommonComponents;

public interface IFileFormRendererType
{
    public string FileName { get; set; }
    public bool IsDirectory { get; set; }
    public bool CheckForTemplates { get; set; }
    public Action<string, ImmutableArray<IFileTemplate>> OnAfterSubmitAction { get; set; }
}