﻿using System.Collections.Immutable;
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

    public int Count => _edits.Count;
    public bool Any => _edits.Any();

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
            new EditWrapper((readRequest, editResult, cancellationToken) =>
                {
                    var editIsPartOfRequest = false;

                    // This edit was done in the boundary of the request
                    if (rowIndexOffset >= readRequest.RowIndexOffset &&
                        rowIndexOffset < readRequest.RowIndexOffset + readRequest.RowCount)
                    {
                        editIsPartOfRequest = true;
                    }

                    var builder = new StringBuilder();
                    var characterIndexForInsertion = characterIndexOffset - readRequest.CharacterIndexOffset;
                    var rowIndexForInsertion = rowIndexOffset - readRequest.RowIndexOffset;
                    
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
                                builder.Append('\r');
                            }
                            
                            builder.Append(character);

                            editResult.DisplacementTimeline.Add(new RowDisplacementValue(rowIndexForInsertion, 1));
                            editResult.AccumulatedRowDisplacement += 1;

                            if (editIsPartOfRequest)
                            {
                                var builtString = builder.ToString();

                                if (characterIndexForInsertion == 0)
                                {
                                    editResult.ContentRows.Insert(rowIndexForInsertion++, builtString);
                                }
                                else
                                {
                                    // Substring the row text at character index for insertion

                                    var rowText = editResult.ContentRows[rowIndexForInsertion];

                                    var splitFirst = rowText.Substring(0, characterIndexForInsertion) 
                                                     + builder.ToString();

                                    var splitSecond = rowText.Substring(characterIndexForInsertion);

                                    editResult.ContentRows[rowIndexForInsertion] = splitFirst;
                                    editResult.ContentRows.Insert(rowIndexForInsertion + 1, splitSecond);
                                }
                            }

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
                        if (editIsPartOfRequest)
                        {
                            // Immutable string replacement
                            editResult.ContentRows[rowIndexForInsertion] = editResult.ContentRows[rowIndexForInsertion]
                                .Insert(characterIndexForInsertion,
                                    builder.ToString());
                        }
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
            new EditWrapper((readRequest, editResult, cancellationToken) =>
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
            new EditWrapper((readRequest, editResult, cancellationToken) =>
                {
                    var editIsPartOfRequest = false;

                    // This edit was done in the boundary of the request
                    if (rowIndexOffset >= readRequest.RowIndexOffset &&
                        rowIndexOffset < readRequest.RowIndexOffset + readRequest.RowCount)
                    {
                        editIsPartOfRequest = true;
                    }

                    int lastIndex;

                    if (rowCount is not null)
                    {
                        lastIndex = rowIndexOffset + rowCount.Value - 1;  
                        
                        lastIndex = lastIndex > editResult.ContentRows.Count - 1
                            ? editResult.ContentRows.Count - 1
                            : lastIndex;
                    }
                    else
                    {
                        lastIndex = rowIndexOffset;

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

                            if (editIsPartOfRequest)
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

                            if (!editResult.ContentRows[i].EndsWith('\n') && (i < editResult.ContentRows.Count - 1))
                            {
                                editResult.ContentRows[i] += editResult.ContentRows[i + 1];

                                editResult.DisplacementTimeline.Add(new RowDisplacementValue(i, -1));
                                editResult.AccumulatedRowDisplacement -= 1;

                                if (editIsPartOfRequest)
                                    editResult.ContentRows.RemoveAt(i + 1);
                            }
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
            new EditWrapper((readRequest, editResult, cancellationToken) =>
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
            new EditWrapper((readRequest, editResult, cancellationToken) =>
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
            new EditWrapper((readRequest, editResult, cancellationToken) =>
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
            new EditWrapper((readRequest, editResult, cancellationToken) =>
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
            new EditWrapper((readRequest, editResult, cancellationToken) =>
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

    public EditResult ApplyEdits(FileHandleReadRequest readRequest, 
        List<string> rows, 
        List<long> virtualCharacterIndexMarkerForStartOfARow)
    {
        EditResult? editResult = null;

        try
        {
            _editsSemaphoreSlim.Wait();

            editResult = new EditResult(rows,
                virtualCharacterIndexMarkerForStartOfARow, 
                new(),
                0);
            
            foreach (var edit in _edits)
            {
                edit.Edit(readRequest, editResult, default);
            }
        }
        finally
        {
            _editsSemaphoreSlim.Release();
        }

        return editResult;
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