using System.Collections.Immutable;
using Fluxor;

namespace BlazorStudio.ClassLib.Store.DropdownCase;

[FeatureState]
public record DropdownStates(ImmutableList<DropdownKey> ActiveDropdownKeys)
{
    public DropdownStates() : this(ImmutableList<DropdownKey>.Empty)
    {
        
    }
}

public record DropdownKey(Guid Guid)
{
    public static DropdownKey NewDropdownKey()
    {
        return new(Guid.NewGuid());
    }
}

public record AddActiveDropdownKeyAction(DropdownKey DropdownKey);
public record RemoveActiveDropdownKeyAction(DropdownKey DropdownKey);
public record ClearActiveDropdownKeysAction;

public class DropdownStatesReducer
{
    [ReducerMethod]
    public static DropdownStates ReduceAddActiveDropdownKeyAction(DropdownStates previousDropdownStates,
        AddActiveDropdownKeyAction addActiveDropdownKeyAction)
    {
        return previousDropdownStates with
        {
            ActiveDropdownKeys = previousDropdownStates.ActiveDropdownKeys
                .Add(addActiveDropdownKeyAction.DropdownKey)
        };
    }
    
    [ReducerMethod]
    public static DropdownStates ReduceRemoveActiveDropdownKeyAction(DropdownStates previousDropdownStates,
        RemoveActiveDropdownKeyAction removeActiveDropdownKeyAction)
    {
        return previousDropdownStates with
        {
            ActiveDropdownKeys = previousDropdownStates.ActiveDropdownKeys
                .Remove(removeActiveDropdownKeyAction.DropdownKey)
        };
    }
    
    [ReducerMethod(typeof(ClearActiveDropdownKeysAction))]
    public static DropdownStates ReduceClearActiveDropdownKeysAction(DropdownStates previousDropdownStates)
    {
        return previousDropdownStates with
        {
            ActiveDropdownKeys = previousDropdownStates.ActiveDropdownKeys
                .Clear()
        };
    }
}



