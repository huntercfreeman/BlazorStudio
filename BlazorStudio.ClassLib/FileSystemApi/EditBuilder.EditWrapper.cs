using System.Collections.Immutable;
using System.IO.MemoryMappedFiles;
using System.Text;
using BlazorStudio.ClassLib.FileSystem.Interfaces;

namespace BlazorStudio.ClassLib.FileSystemApi;

public partial class EditBuilder
{
    private class EditWrapper
    {
        /// <summary>
        /// Takes row content and a cancellation token. Then modifies row content and returns the displacement
        /// of the text that occurred.
        /// </summary>
        /// <param name="editAsync"></param>
        public EditWrapper(Action<FileHandleReadRequest, EditResult, CancellationToken> edit,
            Func<EditResult, CancellationToken, Task> editAsync)
        {
            Edit = edit;
            EditAsync = editAsync;
        }

        public Action<FileHandleReadRequest, EditResult, CancellationToken> Edit { get; }
        public Func<EditResult, CancellationToken, Task> EditAsync { get; }
    }
}