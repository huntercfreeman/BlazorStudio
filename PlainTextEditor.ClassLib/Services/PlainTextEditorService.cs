using BlazorStudio.ClassLib.FileSystem.Interfaces;
using BlazorStudio.ClassLib.Store.PlainTextEditorCase;
using Fluxor;
using PlainTextEditor.ClassLib.Store.PlainTextEditorCase;

namespace PlainTextEditor.ClassLib.Services;

public class PlainTextEditorService : IPlainTextEditorService, IDisposable
{
    private readonly IDispatcher _dispatcher;
    private readonly IState<PlainTextEditorStates> _plainTextEditorStatesWrap;

    private readonly SemaphoreSlim _onPlainTextEditorConstructedActionsSemaphoreSlim = new(1, 1);
    private readonly Dictionary<PlainTextEditorKey, Func<Task>> _onPlainTextEditorConstructedActionMap = new();

    public PlainTextEditorService(IDispatcher dispatcher, IState<PlainTextEditorStates> plainTextEditorStatesWrap)
    {
        _dispatcher = dispatcher;
        _plainTextEditorStatesWrap = plainTextEditorStatesWrap;

        _plainTextEditorStatesWrap.StateChanged += OnPlainTextEditorStatesWrapStateChanged;
    }

    private async void OnPlainTextEditorStatesWrapStateChanged(object? sender, EventArgs e)
    {
        try
        {
            await _onPlainTextEditorConstructedActionsSemaphoreSlim.WaitAsync();

            var onPlainTextEditorConstructedActions = _onPlainTextEditorConstructedActionMap.AsEnumerable();

            foreach (var pair in onPlainTextEditorConstructedActions)
            {
                if (_plainTextEditorStatesWrap.Value.Map.ContainsKey(pair.Key)) 
                {
                    await pair.Value.Invoke();
                    _onPlainTextEditorConstructedActionMap.Remove(pair.Key);
                }
            }
        }
        finally
        {
            _onPlainTextEditorConstructedActionsSemaphoreSlim.Release();
        }
    }

    public async Task ConstructPlainTextEditorAsync(PlainTextEditorKey plainTextEditorKey,
        Func<Task> plainTextEditorWasConstructedCallback)
    {
        try
        {
            await _onPlainTextEditorConstructedActionsSemaphoreSlim.WaitAsync();
            _onPlainTextEditorConstructedActionMap.Add(plainTextEditorKey, plainTextEditorWasConstructedCallback);
        }
        finally
        {
            _onPlainTextEditorConstructedActionsSemaphoreSlim.Release();
        }

        _dispatcher.Dispatch(new ConstructInMemoryPlainTextEditorRecordAction(plainTextEditorKey));
    }
    
    public void DeconstructPlainTextEditor(PlainTextEditorKey plainTextEditorKey)
    {
        _dispatcher.Dispatch(new DeconstructPlainTextEditorRecordAction(plainTextEditorKey));
    }

    public void Dispose()
    {
        _plainTextEditorStatesWrap.StateChanged -= OnPlainTextEditorStatesWrapStateChanged;
    }
}
