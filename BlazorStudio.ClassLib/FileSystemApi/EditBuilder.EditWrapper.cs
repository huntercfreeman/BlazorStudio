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
        public EditWrapper(List<Func<EditResult, CancellationToken, Task>> editsAsync)
        {
            EditsAsync = editsAsync;
        }

        /// <summary>
        /// This is a list because consecutive Edits of the same kind under certain conditions will combine into 1 so to speak.
        /// <br/><br/>
        /// Example: typing characters consecutively will all merge into one <see cref="EditWrapper"/> where <see cref="EditsAsync"/>
        /// contains many Actions.
        /// </summary>
        public List<Func<EditResult, CancellationToken, Task>> EditsAsync { get; }
    }
}