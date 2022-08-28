using Fluxor;

namespace BlazorStudio.ClassLib.Store.EditorCase;

[FeatureState]
public record EditorState(int TabIndex)
{
    public EditorState() : this(0)
    {

    }
}