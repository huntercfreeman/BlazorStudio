namespace BlazorStudio.ClassLib.Store.DialogCase;

public record DialogKey(Guid Guid)
{
    public static DialogKey NewDialogKey()
    {
        return new(Guid.NewGuid());
    }
}

public static class DialogFacts
{
    public static readonly DialogKey InputFileDialogKey = DialogKey.NewDialogKey();
}