namespace BlazorStudio.ClassLib.TextEditor.Enums;

public static class DecorationKindHelper
{
    public static string ToDecorationKindCssStyleString(byte value)
    {
        var decorationKind = (DecorationKind)value;
        
        return decorationKind switch
        {
            DecorationKind.Method => "pte_plain-text-editor-text-token-display-method-declaration",
            DecorationKind.Type => "pte_plain-text-editor-text-token-display-type",
            DecorationKind.AltFlagOne | DecorationKind.Variable => "pte_plain-text-editor-text-token-display-parameter",
            _ => string.Empty
        };
    }
}