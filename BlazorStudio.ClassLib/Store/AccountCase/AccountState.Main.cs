using Fluxor;

namespace BlazorStudio.ClassLib.Store.AccountCase;

/// <param name="GroupKey">Macro filter</param>
/// <param name="ContainerKey">Micro filter</param>
/// <param name="Alias">Personal Alias which is displayed to other users.</param>
[FeatureState]
public partial record AccountState(string GroupKey, string ContainerKey, string Alias)
{
    public const string DEFAULT_ALIAS = "Anonymous";
    
    private AccountState() 
        : this(
            string.Empty,
            Guid.NewGuid().ToString(),
            DEFAULT_ALIAS)
    {
    }
}