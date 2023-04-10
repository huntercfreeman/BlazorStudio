using System.Collections.Immutable;
using BlazorStudio.ClassLib.DotNet;
using BlazorStudio.ClassLib.Nuget;
using Fluxor;
using Microsoft.CodeAnalysis;

namespace BlazorStudio.ClassLib.Store.NuGetPackageManagerCase;

public partial record NuGetPackageManagerState
{    
    public record SetSelectedProjectToModifyAction(IDotNetProject? SelectedProjectToModify);
    public record SetNugetQueryAction(string NugetQuery);
    public record SetIncludePrereleaseAction(bool IncludePrerelease);
    public record SetMostRecentQueryResultAction(ImmutableArray<NugetPackageRecord> QueryResult);
}