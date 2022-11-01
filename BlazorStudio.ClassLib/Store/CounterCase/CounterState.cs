using Fluxor;

namespace BlazorStudio.ClassLib.Store.CounterCase;

[FeatureState]
public record CounterState(int Count)
{
    public CounterState() : this(0)
    {
        
    }
}