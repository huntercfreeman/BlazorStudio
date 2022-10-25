using BlazorStudio.ClassLib.FileSystem.Interfaces;
using BlazorTextEditor.RazorLib.TextEditor;

namespace BlazorStudio.ClassLib.Store.TextEditorResourceCase;

public record SetTextEditorResourceStateAction(
    TextEditorKey TextEditorKey,
    IAbsoluteFilePath AbsoluteFilePath);