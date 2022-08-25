namespace BlazorStudio.ClassLib.Store.PlainTextEditorCase;

public record RichTextEditorOptions
{
    public int FontSizeInPixels { get; init; } = 18;
    public double WidthOfACharacterInPixels { get; init; }
    public double HeightOfARowInPixels { get; init; }
    public double WidthOfEditorInPixels { get; set; }
    public double HeightOfEditorInPixels { get; set; }
}
