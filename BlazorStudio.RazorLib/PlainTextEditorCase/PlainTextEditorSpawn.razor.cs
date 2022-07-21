using BlazorStudio.ClassLib.Services;
using BlazorStudio.ClassLib.Store.PlainTextEditorCase;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.PlainTextEditorCase;

public partial class PlainTextEditorSpawn : ComponentBase, IDisposable
{
    [Inject]
    private IPlainTextEditorService PlainTextEditorService { get; set; } = null!;

    [Parameter]
    public Func<Task>? AfterInitializationCallback { get; set; }

    public PlainTextEditorKey PlainTextEditorKey = PlainTextEditorKey.NewPlainTextEditorKey();
    private bool _plainTextEditorWasInitialized;
    
    protected override void OnInitialized()
    {
        _ = Task.Run(async () => 
            {
                await PlainTextEditorService
                    .ConstructPlainTextEditorAsync(PlainTextEditorKey, 
                        async () => 
                        {
                            _plainTextEditorWasInitialized = true;
                            await InvokeAsync(StateHasChanged);

                            if (AfterInitializationCallback is not null)
                                await AfterInitializationCallback();
                        });
            });

        base.OnInitialized();
    }

    public void Dispose()
    {
        PlainTextEditorService.DeconstructPlainTextEditor(PlainTextEditorKey);
    }
}