using System.Collections.Immutable;
using System.IO.MemoryMappedFiles;
using System.Text;
using BlazorStudio.ClassLib.FileSystem.Interfaces;

namespace BlazorStudio.ClassLib.FileSystemApi;

public partial class FileSystemProvider : IFileSystemProvider
{
    private partial class FileHandle : IFileHandle
    {
        public class EditBuilder
        {
            private readonly SemaphoreSlim _editsSemaphoreSlim = new(1, 1);
            private readonly List<EditWrapper> _edits = new();
            
            private EditBuilder()
            {
                
            }

            /// <summary>
            /// Make an edit that will be seen when reading from the same FileHandle. However,
            /// this will not edit the file on disk. One must <see cref="Save"/> after editing to persist changes.
            /// </summary>
            /// <returns>An EditBuilder to fluently chain multiple edits together.</returns>
            public static EditBuilder Build()
            {
                return new EditBuilder();
            }
            
            /// <summary>
            /// Make an edit that will be seen when reading from the same FileHandle. However,
            /// this will not edit the file on disk. One must <see cref="Save"/> after editing to persist changes.
            /// </summary>
            public EditBuilder Insert(int rowIndexOffset, int characterIndexOffset, int rowCount, int characterCount)
            {
                LockEdit(
                    new EditWrapper(() =>
                        {
                        
                        }, 
                        0, 
                        0));

                return this;
            }
            
            /// <summary>
            /// Make an edit that will be seen when reading from the same FileHandle. However,
            /// this will not edit the file on disk. One must <see cref="Save"/> after editing to persist changes.
            /// </summary>
            public async Task<EditBuilder> InsertAsync(int rowIndexOffset, int characterIndexOffset, int rowCount, int characterCount, 
                CancellationToken cancellationToken)
            {
                await LockEditAsync(
                    new EditWrapper(() =>
                        {
                        
                        }, 
                        0, 
                        0),
                    cancellationToken);

                return this;
            }
            
            /// <summary>
            /// Make an edit that will be seen when reading from the same FileHandle. However,
            /// this will not edit the file on disk. One must <see cref="Save"/> after editing to persist changes.
            /// </summary>
            public EditBuilder Remove(int rowIndexOffset, int characterIndexOffset, int rowCount, int characterCount)
            {
                LockEdit(
                    new EditWrapper(() =>
                        {
                        
                        }, 
                        0, 
                        0));

                return this;
            }
            
            /// <summary>
            /// Make an edit that will be seen when reading from the same FileHandle. However,
            /// this will not edit the file on disk. One must <see cref="Save"/> after editing to persist changes.
            /// </summary>
            public async Task<EditBuilder> RemoveAsync(int rowIndexOffset, int characterIndexOffset, int rowCount, int characterCount, 
                CancellationToken cancellationToken)
            {
                await LockEditAsync(
                    new EditWrapper(() =>
                        {
                        
                        }, 
                        0, 
                        0),
                    cancellationToken);

                return this;
            }
            
            /// <summary>
            /// Make an edit that will be seen when reading from the same FileHandle. However,
            /// this will not edit the file on disk. One must <see cref="Save"/> after editing to persist changes.
            /// </summary>
            public EditBuilder Undo(int rowIndexOffset, int characterIndexOffset, int rowCount, int characterCount)
            {
                LockEdit(
                    new EditWrapper(() =>
                        {
                        
                        }, 
                        0, 
                        0));

                return this;
            }
            
            /// <summary>
            /// Make an edit that will be seen when reading from the same FileHandle. However,
            /// this will not edit the file on disk. One must <see cref="Save"/> after editing to persist changes.
            /// </summary>
            public async Task<EditBuilder> UndoAsync(int rowIndexOffset, int characterIndexOffset, int rowCount, int characterCount, 
                CancellationToken cancellationToken)
            {
                await LockEditAsync(
                    new EditWrapper(() =>
                        {
                        
                        }, 
                        0, 
                        0),
                    cancellationToken);

                return this;
            }
            
            /// <summary>
            /// Make an edit that will be seen when reading from the same FileHandle. However,
            /// this will not edit the file on disk. One must <see cref="Save"/> after editing to persist changes.
            /// </summary>
            public EditBuilder Redo(int rowIndexOffset, int characterIndexOffset, int rowCount, int characterCount)
            {
                LockEdit(
                    new EditWrapper(() =>
                        {
                        
                        }, 
                        0, 
                        0));

                return this;
            }
            
            /// <summary>
            /// Make an edit that will be seen when reading from the same FileHandle. However,
            /// this will not edit the file on disk. One must <see cref="Save"/> after editing to persist changes.
            /// </summary>
            public async Task<EditBuilder> RedoAsync(int rowIndexOffset, int characterIndexOffset, int rowCount, int characterCount, 
                CancellationToken cancellationToken)
            {
                await LockEditAsync(
                    new EditWrapper(() =>
                        {
                        
                        }, 
                        0, 
                        0),
                    cancellationToken);

                return this;
            }
            
            private void LockEdit(EditWrapper editWrapper)
            {
                try
                {
                    _editsSemaphoreSlim.Wait();

                    _edits.Add(editWrapper);
                }
                finally
                {
                    _editsSemaphoreSlim.Release();
                }
            }
            
            private async Task LockEditAsync(EditWrapper editWrapper, CancellationToken cancellationToken)
            {
                try
                {
                    await _editsSemaphoreSlim.WaitAsync(cancellationToken);

                    _edits.Add(editWrapper);
                }
                finally
                {
                    _editsSemaphoreSlim.Release();
                }
            }
            
            public void Clear()
            {
                try
                {
                    _editsSemaphoreSlim.Wait();

                    _edits.Clear();
                }
                finally
                {
                    _editsSemaphoreSlim.Release();
                }
            }
            
            public async Task ClearAsync(CancellationToken cancellationToken)
            {
                try
                {
                    await _editsSemaphoreSlim.WaitAsync(cancellationToken);

                    _edits.Clear();
                }
                finally
                {
                    _editsSemaphoreSlim.Release();
                }
            }

            private class EditWrapper
            {
                public EditWrapper(Action edit, int rowDisplacement, int characterDisplacement)
                {
                    Edit = edit;
                    RowDisplacement = rowDisplacement;
                    CharacterDisplacement = characterDisplacement;
                }

                public Action Edit { get; set; }
                public int RowDisplacement { get; set; }
                public int CharacterDisplacement { get; set; }
            }
        }
    }
}