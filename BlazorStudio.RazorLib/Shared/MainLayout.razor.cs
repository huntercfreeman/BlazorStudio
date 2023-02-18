using BlazorALaCarte.DialogNotification;
using BlazorALaCarte.DialogNotification.Dialog;
using BlazorALaCarte.Shared.Dimensions;
using BlazorALaCarte.Shared.Drag;
using BlazorALaCarte.Shared.Icons;
using BlazorALaCarte.Shared.Options;
using BlazorALaCarte.Shared.Resize;
using BlazorALaCarte.Shared.Store;
using BlazorALaCarte.Shared.Store.DragCase;
using BlazorALaCarte.Shared.Store.ThemeCase;
using BlazorALaCarte.Shared.Theme;
using BlazorStudio.ClassLib.Dimensions;
using BlazorStudio.ClassLib.FileSystem.Classes;
using BlazorStudio.ClassLib.Panel;
using BlazorStudio.ClassLib.Store.FontCase;
using BlazorStudio.ClassLib.Store.PanelCase;
using BlazorStudio.ClassLib.Store.SolutionExplorer;
using BlazorTextEditor.RazorLib;
using Fluxor;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BlazorStudio.RazorLib.Shared;

public partial class MainLayout : LayoutComponentBase, IDisposable
{
    [Inject]
    private IState<DragState> DragStateWrap { get; set; } = null!;
    [Inject]
    private IAppOptionsService AppOptionsService { get; set; } = null!;
    [Inject]
    private IState<FontState> FontStateWrap { get; set; } = null!;
    [Inject]
    private IState<PanelsCollection> PanelsCollectionWrap { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;
    [Inject]
    private IJSRuntime JsRuntime { get; set; } = null!;
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;

    private string _message = string.Empty;
    
    private string UnselectableClassCss => DragStateWrap.Value.ShouldDisplay
        ? "balc_unselectable"
        : string.Empty;
    
    private bool _previousDragStateWrapShouldDisplay;

    private ElementDimensions _bodyElementDimensions = new();
    private ElementDimensions _footerElementDimensions = new();

    protected override void OnInitialized()
    {
        DragStateWrap.StateChanged += DragStateWrapOnStateChanged;
        AppOptionsService.AppOptionsStateWrap.StateChanged += ApplicationOptionsStateWrapOnStateChanged;
        FontStateWrap.StateChanged += FontStateWrapOnStateChanged;
        
        var bodyHeight = _bodyElementDimensions.DimensionAttributes
            .Single(da => da.DimensionAttributeKind == DimensionAttributeKind.Height);
        
        bodyHeight.DimensionUnits.AddRange(new []
        {
            new DimensionUnit
            {
                Value = 78,
                DimensionUnitKind = DimensionUnitKind.Percentage
            },
            new DimensionUnit
            {
                Value = ResizableRow.RESIZE_HANDLE_HEIGHT_IN_PIXELS / 2,
                DimensionUnitKind = DimensionUnitKind.Pixels,
                DimensionOperatorKind = DimensionOperatorKind.Subtract
            },
            new DimensionUnit
            {
                Value = SizeFacts.Bstudio.Header.Height.Value / 2,
                DimensionUnitKind = SizeFacts.Bstudio.Header.Height.DimensionUnitKind,
                DimensionOperatorKind = DimensionOperatorKind.Subtract
            }
        });
        
        base.OnInitialized();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await TextEditorService.GlobalOptionsSetFromLocalStorageAsync();
            
            var fontSizeString = await JsRuntime.InvokeAsync<string>(
                "blazorStudio.localStorageGetItem",
                "bstudio_fontSize");
            
            var iconSizeString = await JsRuntime.InvokeAsync<string>(
                "blazorStudio.localStorageGetItem",
                "bstudio_iconSize");
            
            var themeClassCssString = await JsRuntime.InvokeAsync<string>(
                "blazorStudio.localStorageGetItem",
                "bstudio_themeClassCssString");

            // TODO: BlazorALaCarte
            // var matchedThemeRecord = ThemeFacts.DefaultThemeRecords.FirstOrDefault(x => 
            //     x.ClassCssString == themeClassCssString);
            //
            // if (matchedThemeRecord is not null)
            // {
            //     TextEditorService.SetTheme(matchedThemeRecord.ThemeColorKind == ThemeColorKind.Light
            //         ? ThemeFacts.VisualStudioLightThemeClone
            //         : ThemeFacts.VisualStudioDarkThemeClone);
            //     
            //     Dispatcher.Dispatch(new SetThemeStateAction(matchedThemeRecord));
            // }

            if (int.TryParse(fontSizeString, out var fontSize))
                Dispatcher.Dispatch(new SetFontSizeInPixelsAction(fontSize));
            
            if (int.TryParse(iconSizeString, out var iconSize))
                AppOptionsService.SetIconSize(iconSize);

            if (System.IO.File.Exists("/home/hunter/Repos/Demos/TestApp/TestApp.sln"))
            {
                Dispatcher.Dispatch(new SolutionExplorerState.RequestSetSolutionExplorerStateAction(
                    new AbsoluteFilePath("/home/hunter/Repos/Demos/TestApp/TestApp.sln", false)));
            }
        }
        
        await base.OnAfterRenderAsync(firstRender);
    }

    private void ApplicationOptionsStateWrapOnStateChanged(object? sender, EventArgs e)
    {
        InvokeAsync(StateHasChanged);
    }
    
    private void FontStateWrapOnStateChanged(object? sender, EventArgs e)
    {
        InvokeAsync(StateHasChanged);
    }

    private async void DragStateWrapOnStateChanged(object? sender, EventArgs e)
    {
        if (_previousDragStateWrapShouldDisplay != DragStateWrap.Value.ShouldDisplay)
        {
            _previousDragStateWrapShouldDisplay = DragStateWrap.Value.ShouldDisplay;
            await InvokeAsync(StateHasChanged);
        }
    }
    
    private async Task ReRenderAsync()
    {
        await InvokeAsync(StateHasChanged);
    }

    public void Dispose()
    {
        DragStateWrap.StateChanged -= DragStateWrapOnStateChanged;
        AppOptionsService.AppOptionsStateWrap.StateChanged += ApplicationOptionsStateWrapOnStateChanged;
        FontStateWrap.StateChanged -= FontStateWrapOnStateChanged;
    }
}