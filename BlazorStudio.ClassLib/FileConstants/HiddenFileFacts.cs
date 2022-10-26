using System.Collections.Immutable;

namespace BlazorStudio.ClassLib.FileConstants;

public static class HiddenFileFacts
{
    public const string Bin = "bin";
    public const string Obj = "obj";

    /// <summary>
    ///     If rendering a .csproj file pass in <see cref="ExtensionNoPeriodFacts.C_SHARP_PROJECT" />
    ///     Then perhaps the returning array would contain { "bin", "obj" } as they should be hidden
    ///     with this context.
    /// </summary>
    /// <returns></returns>
    public static ImmutableArray<string> GetHiddenFilesByContainerFileExtension(string extensionNoPeriod)
    {
        return extensionNoPeriod switch
        {
            ExtensionNoPeriodFacts.C_SHARP_PROJECT => new[] { Bin, Obj }.ToImmutableArray(),
            _ => ImmutableArray<string>.Empty,
        };
    }
}