using System.Collections.Immutable;
using BlazorStudio.ClassLib.FileSystem.Classes;
using BlazorStudio.ClassLib.FileSystemApi;
using BlazorStudio.ClassLib.FileSystemApi.MemoryMapped;
using BlazorStudio.ClassLib.Store.SolutionCase;
using Fluxor;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;

namespace BlazorStudio.ClassLib.RoslynHelpers;

public static class FileToDocumentIndexer
{
    public static async Task IndexFilesToDocuments(MSBuildWorkspace? solutionWorkspace, IDispatcher dispatcher,
        CancellationToken cancellationToken)
    {
        if (solutionWorkspace is null)
            return;

        Dictionary<AbsoluteFilePathStringValue, IndexedDocument> localFileDocumentMap = new();
                
        foreach (Project project in solutionWorkspace.CurrentSolution.Projects)
        {
            foreach (Document document in project.Documents)
            {
                if (document.FilePath is not null)
                {
                    var absoluteFilePath = new AbsoluteFilePathDotNet(document.FilePath, false, project.Id);
                    
                    localFileDocumentMap
                        .Add(new AbsoluteFilePathStringValue(absoluteFilePath),
                            new IndexedDocument(document, absoluteFilePath));
                }
            }
        }
        
        dispatcher.Dispatch(new SetSolutionFileDocumentMapAction(localFileDocumentMap.ToImmutableDictionary()));
    }
}