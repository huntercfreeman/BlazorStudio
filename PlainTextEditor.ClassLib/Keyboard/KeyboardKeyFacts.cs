namespace PlainTextEditor.ClassLib.Keyboard;

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
            case "\n":
            case WhitespaceKeys.ENTER_CODE:
            case " ":
            case WhitespaceKeys.SPACE_CODE:
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
        public const string SPACE_CODE = "Space";
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
}
