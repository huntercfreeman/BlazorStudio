using Fluxor;

namespace BlazorStudio.ClassLib.Store.AccountCase;

/// <param name="ContainerKey">Macro filter</param>
/// <param name="GroupKey">Micro filter</param>
/// <param name="Alias">Personal Alias which is displayed to other users.</param>
[FeatureState]
public partial record AccountState(string ContainerKey, string GroupKey, string Alias)
{
    public const string DEFAULT_ALIAS = "Anonymous";
    public const string EMPTY_CONTAINER_KEY = "00000000-0000-0000-0000-000000000000";
    public const string EMPTY_GROUP_KEY = "00000000-0000-0000-0000-000000000000";
    
    private AccountState() 
        : this(
            EMPTY_CONTAINER_KEY,
            EMPTY_GROUP_KEY,
            DEFAULT_ALIAS)
    {
    }
}