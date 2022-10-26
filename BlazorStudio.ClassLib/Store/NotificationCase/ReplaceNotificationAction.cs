namespace BlazorStudio.ClassLib.Store.NotificationCase;

public record ReplaceNotificationAction(NotificationRecord PreviousNotificationRecord,
    NotificationRecord NextNotificationRecord);