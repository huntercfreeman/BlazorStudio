namespace BlazorStudio.ClassLib.Store.AccountCase;

public partial record AccountState
{
    public record AccountStateWithAction(Func<AccountState, AccountState> WithAction);
}