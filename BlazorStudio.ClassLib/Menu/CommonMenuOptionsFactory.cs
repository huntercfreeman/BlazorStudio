using BlazorStudio.ClassLib.CommonComponents;
using BlazorStudio.ClassLib.FileSystem.Interfaces;

namespace BlazorStudio.ClassLib.Menu;

public class CommonMenuOptionsFactory : ICommonMenuOptionsFactory
{
    private readonly ICommonComponentRenderers _commonComponentRenderers;

    public CommonMenuOptionsFactory(ICommonComponentRenderers commonComponentRenderers)
    {
        _commonComponentRenderers = commonComponentRenderers;
    }
    
    public MenuOptionRecord NewEmptyFile(IAbsoluteFilePath parentDirectory)
    {
        return new MenuOptionRecord(
            "New Empty File",
            MenuOptionKind.Create,
            WidgetRendererType: _commonComponentRenderers.FileFormRendererType,
            WidgetParameters: new Dictionary<string, object?>?
            {
                {
                    
                }
            });
    }
/*
 * -New
        -Empty File
        -Templated File
            -Add a Codebehind Prompt when applicable
    -Base file operations
        -Copy
        -Delete
        -Cut
        -Rename
    -Base directory operations
        -Copy
        -Delete
        -Cut
        -Rename
        -Paste
 */
}

public interface ICommonMenuOptionsFactory
{
    public MenuOptionRecord NewEmptyFile(IAbsoluteFilePath parentDirectory);
/*
 * -New
        -Empty File
        -Templated File
            -Add a Codebehind Prompt when applicable
    -Base file operations
        -Copy
        -Delete
        -Cut
        -Rename
    -Base directory operations
        -Copy
        -Delete
        -Cut
        -Rename
        -Paste
 */
}