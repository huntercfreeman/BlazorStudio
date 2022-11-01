namespace BlazorStudio.ClassLib.Store.DialogCase;

public record DialogKey(Guid Guid)
{
    public static DialogKey NewDialogKey()
    {
        return new(Guid.NewGuid());
    }
}