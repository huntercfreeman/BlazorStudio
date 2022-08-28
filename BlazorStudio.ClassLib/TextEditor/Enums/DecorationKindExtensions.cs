namespace BlazorStudio.ClassLib.TextEditor.Enums;

public static class DecorationKindHelper
{
    public static string ToDecorationKindCssStyleString(byte value)
    {
        var decorationKind = (DecorationKind)value;
        
        return decorationKind switch
        {
            DecorationKind.Method => "pte_plain-text-editor-text-token-display-method-declaration",
            _ => string.Empty
        };
    }
}