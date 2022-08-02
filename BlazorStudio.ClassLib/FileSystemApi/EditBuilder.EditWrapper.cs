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
        public EditWrapper(List<Func<FileHandleReadRequest, EditResult, CancellationToken, Task>> editsAsync,
            EditWrapperKind editWrapperKind)
        {
            EditsAsync = editsAsync;
            EditWrapperKind = editWrapperKind;
        }

        /// <summary>
        /// This is a list because consecutive Edits of the same kind under certain conditions will combine into 1 so to speak.
        /// <br/><br/>
        /// Example: typing characters consecutively will all merge into one <see cref="EditWrapper"/> where <see cref="EditsAsync"/>
        /// contains many Actions.
        /// </summary>
        public List<Func<FileHandleReadRequest, EditResult, CancellationToken, Task>> EditsAsync { get; }
        /// <summary>
        /// Consecutive edits to the document of the same <see cref="EditWrapperKind"/> may be combined into one
        /// <see cref="EditWrapper"/> by adding to the previous one's <see cref="EditsAsync"/>.
        /// <br/><br/>
        /// This allows one to <see cref="EditBuilder.UndoAsync"/> many tiny edits that occur consecutively
        /// such as typing a word where each character insertion would otherwise be its own individual EditWrapper.
        /// </summary>
        public EditWrapperKind EditWrapperKind { get; }
    }
}