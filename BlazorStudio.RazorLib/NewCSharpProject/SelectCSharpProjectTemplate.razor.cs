using BlazorStudio.ClassLib.Store.TreeViewCase;
using Fluxor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using static BlazorStudio.RazorLib.NewCSharpProject.NewCSharpProjectDialog;

namespace BlazorStudio.RazorLib.NewCSharpProject;

public partial class SelectCSharpProjectTemplate : ComponentBase, IDisposable
{
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    [Parameter, EditorRequired]
    public List<CSharpTemplate>? Templates { get; set; }
    [Parameter, EditorRequired]
    public Func<Task> ReRenderCallbackFunc { get; set; } = null!;

    private NewCSharpProjectDialog.CSharpTemplate? _selectedCSharpTemplate;
    private bool _forceSelectCSharpTemplateTreeViewOpen;

    private TreeViewWrapKey _newCSharpProjectTreeViewKey = TreeViewWrapKey.NewTreeViewWrapKey();
    
    public NewCSharpProjectDialog.CSharpTemplate? SelectedCSharpTemplate => _selectedCSharpTemplate;

    private List<RenderCSharpTemplate> GetRootThemes()
    {
        return Templates
            .Select(x => new RenderCSharpTemplate
            {
                CSharpTemplate = x,
                IsExpandable = true,
                TitleFunc = () => x.TemplateName
            })
            .ToList();
    }

    private Task<IEnumerable<RenderCSharpTemplate>> GetChildren(RenderCSharpTemplate renderCSharpTemplate)
    {
        var acceptButton = new RenderCSharpTemplate
        {
            CSharpTemplate = renderCSharpTemplate.CSharpTemplate,
            IsExpandable = false,
            TitleFunc = () => "Confirm",
            OnClick = () =>
            {
                _selectedCSharpTemplate = renderCSharpTemplate.CSharpTemplate;
                InvokeAsync(StateHasChanged);
                ReRenderCallbackFunc.Invoke();
            }
        };

        var renderShortName = new RenderCSharpTemplate
        {
            CSharpTemplate = renderCSharpTemplate.CSharpTemplate,
            IsExpandable = false,
            TitleFunc = () => renderCSharpTemplate.CSharpTemplate.ShortName,
            StringIdentifier = "ShortName"
        };

        var renderLanguage = new RenderCSharpTemplate
        {
            CSharpTemplate = renderCSharpTemplate.CSharpTemplate,
            IsExpandable = false,
            TitleFunc = () => renderCSharpTemplate.CSharpTemplate.Language,
            StringIdentifier = "Language"
        };

        var renderTags = new RenderCSharpTemplate
        {
            CSharpTemplate = renderCSharpTemplate.CSharpTemplate,
            IsExpandable = false,
            TitleFunc = () => renderCSharpTemplate.CSharpTemplate.Tags,
            StringIdentifier = "Tags"
        };

        return Task.FromResult(new List<RenderCSharpTemplate>()
        {
            acceptButton,
            renderShortName,
            renderLanguage,
            renderTags
        }.AsEnumerable());
    }

    private void TreeViewOnEnterKeyDown(RenderCSharpTemplate renderCSharpTemplate, Action toggleIsExpanded)
    {
        _selectedCSharpTemplate = renderCSharpTemplate.CSharpTemplate;
        ReRenderCallbackFunc.Invoke();
    }

    private void TreeViewOnSpaceKeyDown(RenderCSharpTemplate renderCSharpTemplate, Action toggleIsExpanded)
    {
        toggleIsExpanded();
    }

    private void TreeViewOnDoubleClick(RenderCSharpTemplate renderCSharpTemplate, Action toggleIsExpanded, MouseEventArgs mouseEventArgs)
    {
        toggleIsExpanded();
    }

    public void Dispose()
    {
        Dispatcher.Dispatch(new DisposeTreeViewWrapAction(_newCSharpProjectTreeViewKey));
    }
}