using System.Collections.Immutable;
using BlazorStudio.ClassLib.FileSystem.Classes;
using BlazorStudio.ClassLib.FileSystem.Interfaces;
using BlazorStudio.ClassLib.FileSystemApi.MemoryMapped;
using BlazorStudio.ClassLib.Store.PlainTextEditorCase;
using BlazorStudio.ClassLib.UserInterface;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;

namespace Experiments.Server.Pages;

public partial class PlainTextEditorExperiments : FluxorComponent
{
    [Inject]
    private IFileSystemProvider FileSystemProvider { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;
    
    private Dimensions _editorDisplayDimensions = new Dimensions
    {
        DimensionsPositionKind = DimensionsPositionKind.Static,
        WidthCalc = new List<DimensionUnit>
        {
            new()
            {
                DimensionUnitKind = DimensionUnitKind.Percentage,
                Value = 100
            }
        },
        HeightCalc = new List<DimensionUnit>
        {
            new()
            {
                DimensionUnitKind = DimensionUnitKind.Percentage,
                Value = 100
            }
        }
    };

    private ImmutableArray<IAbsoluteFilePath> _testingFiles = new IAbsoluteFilePath[]
    {
        //new AbsoluteFilePath("/home/hunter/Documents/TestData/PlainTextEditorStates.Effect.cs", false),
        new AbsoluteFilePath("/home/hunter/Documents/TestData/main.c", false),
    }.ToImmutableArray();

    protected override Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            foreach (var absoluteFilePath in _testingFiles)
            {
                var plainTextEditorKey = PlainTextEditorKey.NewPlainTextEditorKey();

                Dispatcher.Dispatch(
                    new ConstructTokenizedPlainTextEditorRecordAction(plainTextEditorKey,
                        absoluteFilePath,
                        FileSystemProvider,
                        CancellationToken.None)
                );    
            }
        }
        
        return base.OnAfterRenderAsync(firstRender);
    }
}