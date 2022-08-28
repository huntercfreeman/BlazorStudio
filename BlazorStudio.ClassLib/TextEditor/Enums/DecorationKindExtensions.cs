using Microsoft.AspNetCore.Components;

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
            DecorationKind.Keyword => "pte_plain-text-editor-text-token-display-keyword",
            DecorationKind.AltFlagOne | DecorationKind.Variable => "pte_plain-text-editor-text-token-display-parameter",
            DecorationKind.AltFlagOne | DecorationKind.Constant => "pte_plain-text-editor-text-token-display-string-literal",
            DecorationKind.AltFlagTwo | DecorationKind.Method => "pte_plain-text-editor-text-token-display-comment",
            _ => string.Empty
        };
    }
}