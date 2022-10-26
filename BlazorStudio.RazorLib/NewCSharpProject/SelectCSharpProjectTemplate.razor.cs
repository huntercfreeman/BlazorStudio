using BlazorStudio.ClassLib.Store.TreeViewCase;
using BlazorStudio.RazorLib.TreeViewCase;
using Fluxor;
using Microsoft.AspNetCore.Components;
using static BlazorStudio.RazorLib.NewCSharpProject.NewCSharpProjectDialog;

namespace BlazorStudio.RazorLib.NewCSharpProject;

public partial class SelectCSharpProjectTemplate : ComponentBase, IDisposable
{
    private bool _forceSelectCSharpTemplateTreeViewOpen;

    private TreeViewWrapKey _newCSharpProjectTreeViewKey = TreeViewWrapKey.NewTreeViewWrapKey();

    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    [Parameter]
    [EditorRequired]
    public List<CSharpTemplate> Templates { get; set; } = null!;
    [Parameter]
    [EditorRequired]
    public Func<Task> ReRenderCallbackFunc { get; set; } = null!;

    public CSharpTemplate? SelectedCSharpTemplate { get; private set; }

    public void Dispose()
    {
        Dispatcher.Dispatch(new DisposeTreeViewWrapAction(_newCSharpProjectTreeViewKey));
    }

    private List<RenderCSharpTemplate> GetRootThemes()
    {
        return Templates
            .Select(x => new RenderCSharpTemplate(
                x,
                () => x.TemplateName,
                null,
                false,
                null))
            .ToList();
    }

    private Task<IEnumerable<RenderCSharpTemplate>> GetChildren(RenderCSharpTemplate renderCSharpTemplate)
    {
        var acceptButton = new RenderCSharpTemplate(
            renderCSharpTemplate.CSharpTemplate,
            () => "Confirm",
            null,
            false,
            () =>
            {
                SelectedCSharpTemplate = renderCSharpTemplate.CSharpTemplate;
                InvokeAsync(StateHasChanged);
                ReRenderCallbackFunc.Invoke();
            });

        var renderShortName = new RenderCSharpTemplate(
            renderCSharpTemplate.CSharpTemplate,
            () => renderCSharpTemplate.CSharpTemplate.ShortName,
            "ShortName",
            false,
            null);

        var renderLanguage = new RenderCSharpTemplate(
            renderCSharpTemplate.CSharpTemplate,
            () => renderCSharpTemplate.CSharpTemplate.Language,
            "Language",
            false,
            null);

        var renderTags = new RenderCSharpTemplate(
            renderCSharpTemplate.CSharpTemplate,
            () => renderCSharpTemplate.CSharpTemplate.Tags,
            "Tags",
            false,
            null);

        return Task.FromResult(new List<RenderCSharpTemplate>
        {
            acceptButton,
            renderShortName,
            renderLanguage,
            renderTags,
        }.AsEnumerable());
    }

    private void TreeViewOnEnterKeyDown(TreeViewKeyboardEventDto<RenderCSharpTemplate> treeViewKeyboardEventDto)
    {
        SelectedCSharpTemplate = treeViewKeyboardEventDto.Item.CSharpTemplate;
        ReRenderCallbackFunc.Invoke();
    }

    private void TreeViewOnSpaceKeyDown(TreeViewKeyboardEventDto<RenderCSharpTemplate> treeViewKeyboardEventDto)
    {
        treeViewKeyboardEventDto.ToggleIsExpanded.Invoke();
    }

    private void TreeViewOnDoubleClick(TreeViewMouseEventDto<RenderCSharpTemplate> treeViewMouseEventDto)
    {
        treeViewMouseEventDto.ToggleIsExpanded.Invoke();
    }
}