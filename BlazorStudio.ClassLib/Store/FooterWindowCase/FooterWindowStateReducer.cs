using Fluxor;

namespace BlazorStudio.ClassLib.Store.FooterWindowCase;

public class FooterWindowStateReducer
{
    [ReducerMethod]
    public static FooterWindowState ReduceSetActiveFooterWindowKindAction(FooterWindowState previousFooterWindowState,
        SetActiveFooterWindowKindAction setActiveFooterWindowKindAction)
    {
        return new(setActiveFooterWindowKindAction.FooterWindowKind);
    }
}