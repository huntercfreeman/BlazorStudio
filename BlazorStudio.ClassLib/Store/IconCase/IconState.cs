using Fluxor;

namespace BlazorStudio.ClassLib.Store.IconCase;

[FeatureState]
public record IconState(int IconSizeInPixels)
{
    public IconState() : this(18)
    {
        
    }
}