using System.Collections.Immutable;
using System.IO.MemoryMappedFiles;
using System.Text;
using BlazorStudio.ClassLib.FileSystem.Interfaces;

namespace BlazorStudio.ClassLib.FileSystemApi;

public partial class EditBuilder
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

    public EditBuilder Insert(int rowIndexOffset, int characterIndexOffset, string content)
    {
        LockEdit(
            new EditWrapper((contentRows, cancellationToken) =>
                {
                    contentRows[rowIndexOffset] = contentRows[rowIndexOffset]
                        .Insert(characterIndexOffset, content);

                    return (0, content.Length);
                },
                async (contentRows, cancellationToken) =>
                {
                    throw new NotImplementedException();
                }));

        return this;
    }

    public async Task<EditBuilder> InsertAsync(int rowIndexOffset, int characterIndexOffset, string content,
        CancellationToken cancellationToken)
    {
        await LockEditAsync(
            new EditWrapper((contentRows, cancellationToken) =>
                {
                    contentRows[rowIndexOffset] = contentRows[rowIndexOffset]
                        .Insert(characterIndexOffset, content);

                    return default;
                },
                async (contentRows, cancellationToken) =>
                {
                    throw new NotImplementedException();
                }),
            cancellationToken);

        return this;
    }

    public EditBuilder Remove(int rowIndexOffset, int characterIndexOffset, int rowCount, int characterCount)
    {
        LockEdit(
            new EditWrapper((contentRows, cancellationToken) =>
                {
                    var lastIndex = rowIndexOffset + rowCount - 1;

                    lastIndex = lastIndex > contentRows.Count
                        ? contentRows.Count
                        : lastIndex;
                    
                    for (int i = lastIndex; i >= rowIndexOffset; i--)
                    {
                        var row = contentRows[i];

                        if (characterIndexOffset == 0 && characterCount >= row.Length)
                        {
                            contentRows.RemoveAt(i);
                        }
                        else if (characterIndexOffset <= row.Length)
                        {
                            var removeCount = row.Length - characterIndexOffset;

                            contentRows[i] = row.Remove(characterIndexOffset, removeCount);
                        }
                    }

                    return default;
                },
                async (contentRows, cancellationToken) =>
                {
                    throw new NotImplementedException();
                }));

        return this;
    }

    public async Task<EditBuilder> RemoveAsync(int rowIndexOffset, int characterIndexOffset, int rowCount,
        int characterCount,
        CancellationToken cancellationToken)
    {
        await LockEditAsync(
            new EditWrapper((contentRows, cancellationToken) =>
                {
                    throw new NotImplementedException();
                },
                async (contentRows, cancellationToken) =>
                {
                    throw new NotImplementedException();
                }),
            cancellationToken);

        return this;
    }

    public EditBuilder Undo()
    {
        LockEdit(
            new EditWrapper((contentRows, cancellationToken) =>
                {
                    throw new NotImplementedException();
                },
                async (contentRows, cancellationToken) =>
                {
                    throw new NotImplementedException();
                }));

        return this;
    }

    public async Task<EditBuilder> UndoAsync(CancellationToken cancellationToken)
    {
        await LockEditAsync(
            new EditWrapper((contentRows, cancellationToken) =>
                {
                    throw new NotImplementedException();
                },
                async (contentRows, cancellationToken) =>
                {
                    throw new NotImplementedException();
                }),
            cancellationToken);

        return this;
    }

    public EditBuilder Redo()
    {
        LockEdit(
            new EditWrapper((contentRows,  editResult, cancellationToken) =>
                {
                    throw new NotImplementedException();
                },
                async (contentRows, editResult, cancellationToken) =>
                {
                    throw new NotImplementedException();
                }));

        return this;
    }

    public async Task<EditBuilder> RedoAsync(CancellationToken cancellationToken)
    {
        await LockEditAsync(
            new EditWrapper((contentRows, editResult, cancellationToken) =>
                {
                    throw new NotImplementedException();
                },
                async (contentRows, editResult, cancellationToken) =>
                {
                    throw new NotImplementedException();
                }),
            cancellationToken);

        return this;
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

    public List<string> ApplyEdits(int rowIndexOffset, int characterIndexOffset, List<string> rows)
    {
        try
        {
            _editsSemaphoreSlim.Wait();

            var editResult = new EditResult(rows, 
                new(), 
                new());
            
            foreach (var edit in _edits)
            {
                edit.Edit(rows, editResult, default);
            }
        }
        finally
        {
            _editsSemaphoreSlim.Release();
        }

        return rows;
    }
    
    public async Task<List<string>> ApplyEditsAsync(List<string> rows, CancellationToken cancellationToken = default)
    {
        try
        {
            await _editsSemaphoreSlim.WaitAsync(cancellationToken);
        }
        finally
        {
            _editsSemaphoreSlim.Release();
        }
        
        return rows;
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
}