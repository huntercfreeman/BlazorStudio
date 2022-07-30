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

    public EditBuilder Insert(int rowIndexOffset, int characterIndexOffset, string contentToInsert)
    {
        LockEdit(
            new EditWrapper((editResult, cancellationToken) =>
                {
                    var builder = new StringBuilder();
                    var characterIndexForInsertion = characterIndexOffset;
                    var rowIndexForInsertion = rowIndexOffset;
                    
                    var previousCharacterWasCarriageReturn = false;
                    
                    for (int i = 0; i < contentToInsert.Length; i++)
                    {
                        var character = contentToInsert[i];
                        
                        if (character == '\r')
                        {
                            previousCharacterWasCarriageReturn = true;
                            continue;
                        }
                        
                        if (character == '\n')
                        {
                            if (previousCharacterWasCarriageReturn)
                            {
                                builder.Insert(characterIndexForInsertion++, '\r');
                            }
                            
                            builder.Insert(characterIndexForInsertion++, character);

                            editResult.DisplacementTimeline.Add(new RowDisplacementValue(rowIndexForInsertion, 1));
                            editResult.AccumulatedRowDisplacement += 1;

                            editResult.ContentRows.Insert(rowIndexForInsertion++, builder.ToString());

                            for (var index = 0;
                                 index < editResult.VirtualCharacterIndexMarkerForStartOfARow.Count;
                                 index++)
                            {
                                var rowStartMarker = editResult.VirtualCharacterIndexMarkerForStartOfARow[index];
                                
                                editResult.VirtualCharacterIndexMarkerForStartOfARow[index] = rowStartMarker + 1;
                            }

                            builder.Clear();
                            characterIndexForInsertion = 0;
                        }
                        else
                        {
                            builder.Append(character);
                        }

                        previousCharacterWasCarriageReturn = false;
                    }

                    if (builder.Length > 0)
                    {
                        // Immutable string assignment
                        editResult.ContentRows[rowIndexForInsertion - 1] = editResult.ContentRows[rowIndexForInsertion - 1]
                            .Insert(characterIndexForInsertion, 
                                builder.ToString());
                    }
                },
                async (editResult, cancellationToken) =>
                {
                    throw new NotImplementedException();
                }));

        return this;
    }

    public async Task<EditBuilder> InsertAsync(int rowIndexOffset, int characterIndexOffset, string contentToInsert,
        CancellationToken cancellationToken)
    {
        await LockEditAsync(
            new EditWrapper((editResult, cancellationToken) =>
                {
                    Insert(rowIndexOffset, characterIndexOffset, contentToInsert);
                },
                async (editResult, cancellationToken) =>
                {
                    throw new NotImplementedException();
                }),
            cancellationToken);

        return this;
    }

    public EditBuilder Remove(int rowIndexOffset, int characterIndexOffset, int? rowCount = null,
        int? characterCount = null)
    {
        LockEdit(
            new EditWrapper((editResult, cancellationToken) =>
                {
                    var lastIndex = editResult.ContentRows.Count - 1;

                    if (rowCount is not null)
                    {
                        lastIndex = rowIndexOffset + rowCount.Value - 1;  
                        
                        lastIndex = lastIndex > editResult.ContentRows.Count - 1
                            ? editResult.ContentRows.Count - 1
                            : lastIndex;
                    }
                    
                    for (int i = lastIndex; i >= rowIndexOffset; i--)
                    {
                        var row = editResult.ContentRows[i];

                        if (characterIndexOffset == 0 && (characterCount is null || characterCount >= row.Length))
                        {
                            editResult.DisplacementTimeline.Add(new RowDisplacementValue(i, -1));
                            editResult.AccumulatedRowDisplacement -= 1;

                            editResult.ContentRows.RemoveAt(i);
                        }
                        else if (characterIndexOffset <= row.Length)
                        {
                            var removeCount = characterCount ?? row.Length - characterIndexOffset;
                            var availableRemoveCount = row.Length - characterIndexOffset;
                            
                            removeCount = removeCount > availableRemoveCount
                                ? availableRemoveCount
                                : removeCount;

                            editResult.ContentRows[i] = row.Remove(characterIndexOffset, removeCount);
                        }
                    }
                },
                async (editResult, cancellationToken) =>
                {
                    throw new NotImplementedException();
                }));

        return this;
    }

    public async Task<EditBuilder> RemoveAsync(int rowIndexOffset, int characterIndexOffset, int? rowCount = null,
        int? characterCount = null, CancellationToken cancellationToken = default)
    {
        await LockEditAsync(
            new EditWrapper((editResult, cancellationToken) =>
                {
                    throw new NotImplementedException();
                },
                async (editResult, cancellationToken) =>
                {
                    throw new NotImplementedException();
                }),
            cancellationToken);

        return this;
    }

    public EditBuilder Undo()
    {
        LockEdit(
            new EditWrapper((editResult, cancellationToken) =>
                {
                    throw new NotImplementedException();
                },
                async (editResult, cancellationToken) =>
                {
                    throw new NotImplementedException();
                }));

        return this;
    }

    public async Task<EditBuilder> UndoAsync(CancellationToken cancellationToken)
    {
        await LockEditAsync(
            new EditWrapper((editResult, cancellationToken) =>
                {
                    throw new NotImplementedException();
                },
                async (editResult, cancellationToken) =>
                {
                    throw new NotImplementedException();
                }),
            cancellationToken);

        return this;
    }

    public EditBuilder Redo()
    {
        LockEdit(
            new EditWrapper((editResult, cancellationToken) =>
                {
                    throw new NotImplementedException();
                },
                async (editResult, cancellationToken) =>
                {
                    throw new NotImplementedException();
                }));

        return this;
    }

    public async Task<EditBuilder> RedoAsync(CancellationToken cancellationToken)
    {
        await LockEditAsync(
            new EditWrapper((editResult, cancellationToken) =>
                {
                    throw new NotImplementedException();
                },
                async (editResult, cancellationToken) =>
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

    public List<string> ApplyEdits(FileHandleReadRequest readRequest, 
        List<string> rows, 
        List<long> virtualCharacterIndexMarkerForStartOfARow)
    {
        try
        {
            _editsSemaphoreSlim.Wait();

            var editResult = new EditResult(rows,
                virtualCharacterIndexMarkerForStartOfARow, 
                new(),
                0);
            
            foreach (var edit in _edits)
            {
                edit.Edit(editResult, default);
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