using BlazorStudio.ClassLib.FileSystem.Interfaces;
using Fluxor;

namespace BlazorStudio.ClassLib.Store.EditorCase;

[FeatureState]
public record EditorState(IAbsoluteFilePath? OpenedAbsoluteFilePath)
{
    public EditorState() : this(default(IAbsoluteFilePath?))
    {

    }
}