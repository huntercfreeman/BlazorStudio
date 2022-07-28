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
        public EditWrapper(
            Func<List<string>, CancellationToken, Task<(int rowDisplacement, int characterDisplacement)>> editAsync)
        {
            EditAsync = editAsync;
        }

        public Func<List<string>, CancellationToken, Task> EditAsync { get; set; }
        public int RowDisplacement { get; set; }
        public int CharacterDisplacement { get; set; }
    }
}