using System.Collections.Immutable;
using Fluxor;

namespace BlazorStudio.ClassLib.Store.NotificationCase;

[FeatureState]
public record NotificationState(ImmutableList<NotificationRecord> Notifications)
{
    public NotificationState() 
        : this(ImmutableList<NotificationRecord>.Empty)
    {
        
    }

    public record RegisterNotificationAction(NotificationRecord NotificationRecord);
    public record DisposeNotificationAction(NotificationKey NotificationKey);

    private static class NotificationStateReducer
    {
        [ReducerMethod]
        public static NotificationState ReduceRegisterNotificationAction(
            NotificationState inNotificationState,
            RegisterNotificationAction registerNotificationAction)
        {
            var nextList = inNotificationState.Notifications
                .Add(registerNotificationAction.NotificationRecord);

            return inNotificationState with
            {
                Notifications = nextList
            };
        }
        
        [ReducerMethod]
        public static NotificationState ReduceDisposeNotificationAction(
            NotificationState inNotificationState,
            DisposeNotificationAction disposeNotificationAction)
        {
            var notification = inNotificationState.Notifications
                .FirstOrDefault(x => 
                    x.NotificationKey == disposeNotificationAction.NotificationKey);

            if (notification is null)
                return inNotificationState;
            
            var nextList = inNotificationState.Notifications
                .Remove(notification);

            return inNotificationState with
            {
                Notifications = nextList
            };
        }
    }
}