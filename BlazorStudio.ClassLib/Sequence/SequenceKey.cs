namespace BlazorStudio.ClassLib.Sequence;

public record SequenceKey(Guid Guid)
{
    public static SequenceKey NewSequenceKey()
    {
        return new SequenceKey(Guid.NewGuid());
    }
}
