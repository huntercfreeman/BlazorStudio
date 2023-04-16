using System.Collections.Immutable;
using BlazorStudio.ClassLib.DotNet;
using BlazorStudio.ClassLib.Nuget;
using Fluxor;

namespace BlazorStudio.ClassLib.Store.NugetPackageManagerCase;

[FeatureState]
public partial record NuGetPackageManagerState(
    IDotNetProject? SelectedProjectToModify, 
    string NugetQuery,
    bool IncludePrerelease,
    ImmutableArray<NugetPackageRecord> MostRecentQueryResult)
{
    public NuGetPackageManagerState() 
        : this(
            default(IDotNetProject?), 
            string.Empty, 
            false, 
            ImmutableArray<NugetPackageRecord>.Empty)
    {
        
    }
}