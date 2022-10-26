using BlazorStudio.ClassLib.Store.DialogCase;
using Fluxor;
using Microsoft.AspNetCore.Components;
using Microsoft.CodeAnalysis;

namespace BlazorStudio.RazorLib.SyntaxRootRender;

public partial class SyntaxNodeTreeViewDialogEntryPoint : ComponentBase
{
    private DialogRecord? _syntaxNodeTreeViewDialog;
    [Inject]
    private IState<DialogStates> DialogStatesWrap { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    [Parameter]
    [EditorRequired]
    public SyntaxNode? SyntaxNode { get; set; }
    [Parameter]
    [EditorRequired]
    public Func<Task<SyntaxNode>>? GetSyntaxNodeFuncAsync { get; set; }
    [Parameter]
    [EditorRequired]
    public string? TitleOverride { get; set; }

    private async Task OpenSyntaxNodeDialogOnClickAsync()
    {
        SyntaxNode targetSyntaxNode;

        if (SyntaxNode is not null)
            targetSyntaxNode = SyntaxNode;
        else if (GetSyntaxNodeFuncAsync is not null)
            targetSyntaxNode = await GetSyntaxNodeFuncAsync();
        else
        {
            throw new ApplicationException($"Must provide either: {nameof(SyntaxNode)} parameter," +
                                           $" or {nameof(GetSyntaxNodeFuncAsync)} parameter.");
        }

        _syntaxNodeTreeViewDialog = new DialogRecord(
            DialogKey.NewDialogKey(),
            TitleOverride ?? "SyntaxNode TreeView",
            typeof(SyntaxNodeTreeView),
            new Dictionary<string, object?>
            {
                {
                    nameof(SyntaxNodeTreeView.SyntaxNode),
                    targetSyntaxNode
                },
            }
        );

        if (DialogStatesWrap.Value.List.All(x => x.DialogKey != _syntaxNodeTreeViewDialog.DialogKey))
            Dispatcher.Dispatch(new RegisterDialogAction(_syntaxNodeTreeViewDialog));
    }
}