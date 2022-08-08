namespace BlazorStudio.ClassLib.Store.NotificationCase;

public record NotificationRecord(NotificationKey NotificationKey,
    string Title,
    Type Type,
    Dictionary<string, object?>? Parameters);