using Fluxor;

namespace BlazorStudio.ClassLib.Store.AccountCase;

public partial record AccountState
{
    private class Reducer
    {
        [ReducerMethod]
        public static AccountState ReduceAccountStateWithAction(
            AccountState inAccountState,
            AccountStateWithAction accountStateWithAction)
        {
            return accountStateWithAction.WithAction
                .Invoke(inAccountState);
        }
    }
}