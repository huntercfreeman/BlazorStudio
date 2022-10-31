using Fluxor;

namespace BlazorStudio.ClassLib.Store.FontCase;

[FeatureState]
public record FontState(int FontSizeInPixels)
{
    public FontState() : this(20)
    {
        
    }
}