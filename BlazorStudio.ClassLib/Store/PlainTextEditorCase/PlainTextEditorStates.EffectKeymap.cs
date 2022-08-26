using System.Collections.Concurrent;
using System.Collections.Immutable;
using BlazorStudio.ClassLib.Clipboard;
using BlazorStudio.ClassLib.FileConstants;
using BlazorStudio.ClassLib.FileSystem.Classes;
using BlazorStudio.ClassLib.FileSystemApi;
using BlazorStudio.ClassLib.FileSystemApi.MemoryMapped;
using BlazorStudio.ClassLib.Keyboard;
using BlazorStudio.ClassLib.RoslynHelpers;
using BlazorStudio.ClassLib.Sequence;
using BlazorStudio.ClassLib.Store.KeyDownEventCase;
using BlazorStudio.ClassLib.Store.SolutionCase;
using BlazorStudio.ClassLib.TaskModelManager;
using BlazorStudio.ClassLib.Virtualize;
using Fluxor;
using Microsoft.AspNetCore.Components;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

namespace BlazorStudio.ClassLib.Store.PlainTextEditorCase;

public partial record PlainTextEditorStates
{
    public class PlainTextEditorStatesEffectKeymap
    {
        private readonly IState<PlainTextEditorStates> _plainTextEditorStatesWrap;
        private readonly IState<SolutionState> _solutionStateWrap;
        private readonly IClipboardProvider _clipboardProvider;
        private readonly ConcurrentQueue<Func<Task>> _handleEffectQueue = new();
        private readonly SemaphoreSlim _executeHandleEffectSemaphoreSlim = new(1, 1);
        
        private SemaphoreSlim __updateTokenSemanticDescriptionsSemaphoreSlim = new(1, 1);

        public PlainTextEditorStatesEffectKeymap(IState<PlainTextEditorStates> plainTextEditorStatesWrap,
            IState<SolutionState> solutionStateWrap,
            IClipboardProvider clipboardProvider)
        {
            _plainTextEditorStatesWrap = plainTextEditorStatesWrap;
            _solutionStateWrap = solutionStateWrap;
            _clipboardProvider = clipboardProvider;
        }

        private async Task QueueHandleEffectAsync(Func<Task> func)
        {
            _handleEffectQueue.Enqueue(func);

            try
            {
                await _executeHandleEffectSemaphoreSlim.WaitAsync();

                if (_handleEffectQueue.TryDequeue(out var fifoHandleEffect))
                {
                    await fifoHandleEffect!.Invoke();
                }
            }
            finally
            {
                _executeHandleEffectSemaphoreSlim.Release();
            }
        }

        /// <summary>
        /// Extremely large files may need this logic as perhaps it is not a good idea
        /// to hold them in memory.
        /// </summary>
        [EffectMethod]
        public async Task HandlePlainTextEditorOnSaveRequested(PlainTextEditorOnSaveRequestedAction plainTextEditorOnSaveRequestedAction,
            IDispatcher dispatcher)
        {
            var previousPlainTextEditorStates = _plainTextEditorStatesWrap.Value;

            var plainTextEditor = previousPlainTextEditorStates
                .Map[plainTextEditorOnSaveRequestedAction.PlainTextEditorKey];

            await plainTextEditor.FileHandle.SaveAsync(
                plainTextEditor.GetDocumentPlainText(), 
                plainTextEditorOnSaveRequestedAction.CancellationToken);
        }
    }
}

