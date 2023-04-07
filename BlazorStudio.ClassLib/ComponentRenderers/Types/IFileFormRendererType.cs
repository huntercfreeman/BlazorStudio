using System.Collections.Immutable;
using BlazorStudio.ClassLib.FileTemplates;

namespace BlazorStudio.ClassLib.ComponentRenderers.Types;

public interface IFileFormRendererType
{
    public string FileName { get; set; }
    public bool IsDirectory { get; set; }
    public bool CheckForTemplates { get; set; }
    public Action<string, IFileTemplate?, ImmutableArray<IFileTemplate>> OnAfterSubmitAction { get; set; }
}