using Fluxor;

namespace BlazorStudio.ClassLib.Store.NotificationCase;

public class NotificationStatesReducer
{
    [ReducerMethod]
    public static NotificationStates ReduceRegisterNotificationAction(NotificationStates previousNotificationStates,
        RegisterNotificationAction registerNotificationAction)
    {
        return new NotificationStates(previousNotificationStates.List
            .Add(registerNotificationAction.NotificationRecord));
    }

    [ReducerMethod]
    public static NotificationStates ReduceDisposeNotificationAction(NotificationStates previousNotificationStates,
        DisposeNotificationAction disposeNotificationAction)
    {
        return new NotificationStates(previousNotificationStates.List
            .Remove(disposeNotificationAction.NotificationRecord));
    }

    [ReducerMethod]
    public static NotificationStates ReduceReplaceNotificationAction(NotificationStates previousNotificationStates,
        ReplaceNotificationAction replaceNotificationAction)
    {
        return new NotificationStates(previousNotificationStates.List
            .Replace(replaceNotificationAction.PreviousNotificationRecord, replaceNotificationAction.NextNotificationRecord));
    }
}