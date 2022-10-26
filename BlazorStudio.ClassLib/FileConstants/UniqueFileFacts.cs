using System.Collections.Immutable;

namespace BlazorStudio.ClassLib.FileConstants;

public static class UniqueFileFacts
{
    public const string PROPERTIES = "Properties";
    public const string WWW_ROOT = "wwwroot";

    /// <summary>
    ///     If rendering a .csproj file pass in <see cref="ExtensionNoPeriodFacts.C_SHARP_PROJECT" />
    ///     Then perhaps the returning array would contain { "Properties", "wwwroot" } as they are unique files
    ///     with this context.
    /// </summary>
    /// <returns></returns>
    public static ImmutableArray<string> GetUniqueFilesByContainerFileExtension(string extensionNoPeriod)
    {
        return extensionNoPeriod switch
        {
            ExtensionNoPeriodFacts.C_SHARP_PROJECT => new[] { PROPERTIES, WWW_ROOT }.ToImmutableArray(),
            _ => ImmutableArray<string>.Empty,
        };
    }
}