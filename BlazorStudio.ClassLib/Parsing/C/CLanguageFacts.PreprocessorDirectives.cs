using System.Collections.Immutable;

namespace BlazorStudio.ClassLib.Parsing.C;

public partial class CLanguageFacts
{
    /// <summary>
    /// When naming the preprocessor directive constants
    /// the starting "#" or "__" must be included in order
    /// to uniquely identify the preprocessor directive.
    /// <br/><br/>
    /// For example take the two directives: "# macro operator" "## macro operator".
    /// <br/><br/>
    /// The name "MACRO_OPERATOR" in this case would appear twice so both the
    /// hashtag, and amount of them need be included in the constant's name to be unique.
    /// <br/><br/>
    /// Seemingly the "macro operator" case is the only case of constant name duplication
    /// if one were to not include the starting "#" or "__".
    /// <br/><br/>
    /// If for whatever reason in the future more preprocessors were to be added however
    /// I don't want to encounter constants having the same name.
    /// <br/><br/>
    /// "#" -> "HT"<br/>
    /// "_" -> "US"<br/>
    /// "double of any symbol" -> "D" followed by the symbol's respective shorthand<br/>
    /// </summary>
    public class PreprocessorDirectives
    {
        public const string HT_INCLUDE = "#include";
        public const string HT_DEFINE = "#define";
        public const string HT_UNDEF = "#undef";
        public const string HT_IF = "#if";
        public const string HT_IFDEF = "#ifdef";
        public const string HT_IFNDEF = "#ifndef";
        public const string HT_ERROR = "#error";
        public const string DUS_FILE = "__FILE__";
        public const string DUS_LINE = "__LINE__";
        public const string DUS_DATE = "__DATE__";
        public const string DUS_TIME = "__TIME__";
        public const string DUS_TIMESTAMP = "__TIMESTAMP__";
        public const string PRAGMA = "pragma";
        public const string HT_MACRO_OPERATOR = "# macro operator";
        public const string DHT_MACRO_OPERATOR = "## macro operator";

        public static readonly ImmutableArray<string> All = new[]
        {
            HT_INCLUDE,
            HT_DEFINE,
            HT_UNDEF,
            HT_IF,
            HT_IFDEF,
            HT_IFNDEF,
            HT_ERROR,
            DUS_FILE,
            DUS_LINE,
            DUS_DATE,
            DUS_TIME,
            DUS_TIMESTAMP,
            PRAGMA,
            HT_MACRO_OPERATOR,
            DHT_MACRO_OPERATOR,
        }.ToImmutableArray();
    }
}