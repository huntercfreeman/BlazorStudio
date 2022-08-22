using System.Collections.Immutable;

namespace BlazorStudio.ClassLib.Keyboard;

public static class KeyboardKeyFacts
{
    public static bool IsMetaKey(KeyDownEventRecord onKeyDownEventArgs)
    {
        if (onKeyDownEventArgs.Key.Length > 1)
            return true;

        return false;

        // TODO: Is a switch needed?
        // switch (onKeyDownEventArgs.Code)
        // {
        // 	default:
        // 		return false;
        // }
    }

    public static bool IsWhitespaceKey(KeyDownEventRecord onKeyDownEventArgs)
    {
        switch (onKeyDownEventArgs.Code)
        {
            case "\t":
            case WhitespaceKeys.TAB_CODE:
            case "\r":
            case WhitespaceKeys.CARRIAGE_RETURN_NEW_LINE_CODE:
            case "\n":
            case WhitespaceKeys.ENTER_CODE:
            case " ":
            case WhitespaceKeys.SPACE_CODE:
                return true;
            default:
                return false;
        }
    }
    
    public static bool IsPunctuationKey(KeyDownEventRecord onKeyDownEventArgs)
    {
        switch (onKeyDownEventArgs.Key)
        {
            case PunctuationKeys.OpenCurlyBrace:
            case PunctuationKeys.CloseCurlyBrace:
            case PunctuationKeys.OpenParenthesis:
            case PunctuationKeys.CloseParenthesis:
            case PunctuationKeys.OpenSquareBracket:
            case PunctuationKeys.CloseSquareBracket:
            case PunctuationKeys.Period:
            case PunctuationKeys.Semicolon:
            case PunctuationKeys.Equal:
            case PunctuationKeys.DoubleQuote:
            case PunctuationKeys.SingleQuote:
            case PunctuationKeys.OpenArrowBracket:
            case PunctuationKeys.EndArrowBracket:
            case PunctuationKeys.ForwardSlash:
            case PunctuationKeys.BackSlash:
                return true;
            default:
                return false;
        }
    }

    public static class MetaKeys
    {
        public const string BACKSPACE_KEY = "Backspace";
        public const string ESCAPE_KEY = "Escape";
        public const string DELETE_KEY = "Delete";
        public const string F10_KEY = "F10";
    }
    
    public static class WhitespaceKeys
    {
        public const string TAB_CODE = "Tab";
        public const string ENTER_CODE = "Enter";
        public const string CARRIAGE_RETURN_NEW_LINE_CODE = "CarriageReturnNewLine";
        public const string SPACE_CODE = "Space";
    }
    
    public static class PunctuationKeys
    {
        public const string OpenCurlyBrace = "{";
        public const string CloseCurlyBrace = "}";
        public const string OpenParenthesis = "(";
        public const string CloseParenthesis = ")";
        public const string OpenSquareBracket = "[";
        public const string CloseSquareBracket = "]";
        public const string Period = ".";
        public const string Semicolon = ";";
        public const string Equal = "=";
        public const string DoubleQuote = "\"";
        public const string SingleQuote = "'";
        public const string OpenArrowBracket = "<";
        public const string EndArrowBracket = ">";
        public const string ForwardSlash = "/";
        public const string BackSlash = "\\";
    }
    
    public static class NewLineCodes
    {
        public const string ENTER_CODE = WhitespaceKeys.ENTER_CODE;
        public const string CARRIAGE_RETURN_NEW_LINE_CODE = WhitespaceKeys.CARRIAGE_RETURN_NEW_LINE_CODE;

        public static readonly ImmutableArray<string> ALL_NEW_LINE_CODES = new string[]
        {
            ENTER_CODE,
            CARRIAGE_RETURN_NEW_LINE_CODE
        }.ToImmutableArray();
    }

    public static class MovementKeys
    {
        public const string ARROW_LEFT_KEY = "ArrowLeft";
        public const string ARROW_DOWN_KEY = "ArrowDown";
        public const string ARROW_UP_KEY = "ArrowUp";
        public const string ARROW_RIGHT_KEY = "ArrowRight";
        public const string HOME_KEY = "Home";
        public const string END_KEY = "End";
    }

    public static class AlternateMovementKeys
    {
        public const string ARROW_LEFT_KEY = "h";
        public const string ARROW_DOWN_KEY = "j";
        public const string ARROW_UP_KEY = "k";
        public const string ARROW_RIGHT_KEY = "l";
    }

    public static bool CheckIsAlternateContextMenuEvent(KeyDownEventRecord keyDownEventRecord)
    {
        string keyOne = "F10";
        string keyTwo = "f10";

        return (keyDownEventRecord.Key == keyOne || keyDownEventRecord.Key == keyTwo)
               && keyDownEventRecord.ShiftWasPressed;
    }
    
    public static bool CheckIsAlternateContextMenuEvent(string key, string code, bool shiftWasPressed, bool altWasPressed)
    {
        string keyOne = "F10";
        string keyTwo = "f10";

        var wasShiftF10 = (key == keyOne || key == keyTwo)  
                          && shiftWasPressed;
        
        var wasAltPeriod = (key == ".")
                          && altWasPressed;

        return wasShiftF10 || wasAltPeriod;
    }
    
    public static bool CheckIsContextMenuEvent(string key, string code, bool shiftWasPressed, bool altWasPressed)
    {
        string keyOne = "ContextMenu";

        return key == keyOne ||
               CheckIsAlternateContextMenuEvent(key, code, shiftWasPressed, altWasPressed);
    }

    public static bool IsMovementKey(KeyDownEventRecord onKeyDownEventArgs)
    {
        switch (onKeyDownEventArgs.Key)
        {
            case MovementKeys.ARROW_LEFT_KEY:
            case MovementKeys.ARROW_DOWN_KEY:
            case MovementKeys.ARROW_UP_KEY:
            case MovementKeys.ARROW_RIGHT_KEY:
            case MovementKeys.HOME_KEY:
            case MovementKeys.END_KEY:
                return true;
            default:
                return false;
        }
    }
    
    public static bool IsAlternateMovementKey(KeyDownEventRecord onKeyDownEventArgs)
    {
        switch (onKeyDownEventArgs.Key)
        {
            case AlternateMovementKeys.ARROW_LEFT_KEY:
            case AlternateMovementKeys.ARROW_DOWN_KEY:
            case AlternateMovementKeys.ARROW_UP_KEY:
            case AlternateMovementKeys.ARROW_RIGHT_KEY:
                return true;
            default:
                return false;
        }
    }
}
