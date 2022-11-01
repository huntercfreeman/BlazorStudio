namespace BlazorStudio.ClassLib.Store.NotificationCase;

public record NotificationRecord(
    NotificationKey NotificationKey,
    string Title,
    Type RendererType,
    Dictionary<string, object?>? Parameters);