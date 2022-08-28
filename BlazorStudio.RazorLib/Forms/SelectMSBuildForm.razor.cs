using BlazorStudio.ClassLib.Store.NotificationCase;
using BlazorStudio.ClassLib.Store.TreeViewCase;
using BlazorStudio.RazorLib.TreeViewCase;
using Fluxor;
using Microsoft.AspNetCore.Components;
using Microsoft.Build.Locator;

namespace BlazorStudio.RazorLib.Forms;

public partial class SelectMSBuildForm : ComponentBase
{
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;
    
    [CascadingParameter]
    public NotificationRecord NotificationRecord { get; set; } = null!;
    
    [Parameter] 
    public VisualStudioInstance[] VisualStudioInstances { get; set; } = null!;

    private TreeViewWrapKey _selectMSBuildTreeViewKey = TreeViewWrapKey.NewTreeViewWrapKey();
    private VisualStudioInstance _selectedVisualStudioInstance;
    private string _providedAbsoluteFilePathToMSBuildVersion = string.Empty;

    protected override void OnParametersSet()
    {
        if (VisualStudioInstances.Any())
        {
            _selectedVisualStudioInstance = VisualStudioInstances[0];
        }
        
        base.OnParametersSet();
    }

    private Task<IEnumerable<VisualStudioInstance>> LoadVisualStudioInstancesChildren(VisualStudioInstance themeKey)
    {
        return Task.FromResult(Array.Empty<VisualStudioInstance>().AsEnumerable());
    }

    private void ThemeTreeViewOnEnterKeyDown(TreeViewKeyboardEventDto<VisualStudioInstance> treeViewKeyboardEventDto)
    {
        _providedAbsoluteFilePathToMSBuildVersion = treeViewKeyboardEventDto.Item.MSBuildPath;
    }

    private void ThemeTreeViewOnSpaceKeyDown(TreeViewKeyboardEventDto<VisualStudioInstance> treeViewKeyboardEventDto)
    {
        _providedAbsoluteFilePathToMSBuildVersion = treeViewKeyboardEventDto.Item.MSBuildPath;
    }

    private void ThemeTreeViewOnDoubleClick(TreeViewMouseEventDto<VisualStudioInstance> treeViewMouseEventDto)
    {
        _providedAbsoluteFilePathToMSBuildVersion = treeViewMouseEventDto.Item.MSBuildPath;
    }
}