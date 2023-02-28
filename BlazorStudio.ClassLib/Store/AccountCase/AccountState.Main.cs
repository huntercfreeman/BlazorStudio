using Fluxor;

namespace BlazorStudio.ClassLib.Store.AccountCase;

/// <param name="ContainerName">Macro filter</param>
/// <param name="GroupName">Micro filter</param>
/// <param name="Alias">Personal Alias which is displayed to other users.</param>
[FeatureState]
public partial record AccountState(string ContainerName, string GroupName, string Alias)
{
    public const string DEFAULT_CONTAINER_NAME = "default-container-name";
    public const string DEFAULT_GROUP_NAME = "default-group-name";
    public const string DEFAULT_ALIAS = "Anonymous";
    public const string SUB_CLAIM_NAME = "sub";
    public const char DIRECTORY_SEPARATOR_CHAR = '/';
    
    private AccountState() 
        : this(
            DEFAULT_CONTAINER_NAME,
            DEFAULT_GROUP_NAME,
            DEFAULT_ALIAS)
    {
    }
}