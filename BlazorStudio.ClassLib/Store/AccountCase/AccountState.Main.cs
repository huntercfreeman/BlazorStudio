using Fluxor;

namespace BlazorStudio.ClassLib.Store.AccountCase;

/// <param name="ContainerName">Macro filter</param>
/// <param name="GroupName">Micro filter</param>
/// <param name="Alias">Personal Alias which is displayed to other users.</param>
[FeatureState]
public partial record AccountState(string ContainerName, string GroupName, string Alias)
{
    public const string DEFAULT_ALIAS = "Anonymous";
    public const string EMPTY_CONTAINER_NAME = "00000000-0000-0000-0000-000000000000";
    public const string EMPTY_GROUP_NAME = "00000000-0000-0000-0000-000000000000";
    
    private AccountState() 
        : this(
            EMPTY_CONTAINER_NAME,
            EMPTY_GROUP_NAME,
            DEFAULT_ALIAS)
    {
    }
}