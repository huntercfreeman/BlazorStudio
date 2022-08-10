using BlazorStudio.ClassLib.Store.ThemeCase;
using BlazorStudio.ClassLib.Store.TreeViewCase;
using Fluxor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.CodeAnalysis;

namespace BlazorStudio.RazorLib.SyntaxRootRender;

public partial class SyntaxRootDisplay : ComponentBase
{
    [Parameter]
    public List<(SyntaxNode syntaxNode, string? fileName)> SyntaxNodeTuples { get; set; } = null!;

    private TreeViewWrapKey _syntaxRootTreeViewKey = TreeViewWrapKey.NewTreeViewWrapKey();
    private GeneralSyntax[] TreeViewRoot => GetTreeViewRoots();

    private GeneralSyntax[] GetTreeViewRoots()
    {
        return SyntaxNodeTuples
            .Select(x => new GeneralSyntax
            {
                Item = x.syntaxNode,
                GeneralSyntaxKind = GeneralSyntaxKind.Node,
                FileName = x.fileName
            })
            .ToArray();
    }

    private Task<IEnumerable<GeneralSyntax>> LoadChildren(GeneralSyntax generalSyntax)
    {
        if (generalSyntax.GeneralSyntaxKind == GeneralSyntaxKind.Node)
        {
            var syntaxNode = (SyntaxNode)(generalSyntax.Item);

            var childNodes = syntaxNode
                .ChildNodes()
                .Select(x => new GeneralSyntax
                {
                    Item = x,
                    GeneralSyntaxKind = GeneralSyntaxKind.Node
                });
            
            var childTokens = syntaxNode
                .ChildTokens()
                .Select(x => new GeneralSyntax
                {
                    Item = x,
                    GeneralSyntaxKind = GeneralSyntaxKind.Token
                });

            return Task.FromResult(childNodes.Union(childTokens));
        }
        else
        {
            return Task.FromResult(Enumerable.Empty<GeneralSyntax>());
        }
    }

    private void TreeViewOnEnterKeyDown(GeneralSyntax generalSyntax, Action toggleIsExpanded)
    {
        toggleIsExpanded();
    }

    private void TreeViewOnSpaceKeyDown(GeneralSyntax generalSyntax, Action toggleIsExpanded)
    {
        toggleIsExpanded();
    }

    private void TreeViewOnDoubleClick(GeneralSyntax generalSyntax, Action toggleIsExpanded, MouseEventArgs mouseEventArgs)
    {
        toggleIsExpanded();
    }

    private class GeneralSyntax
    {
        public GeneralSyntaxKind GeneralSyntaxKind { get; set; }
        public object Item { get; set; }
        public string? FileName { get; set; }
    }

    private enum GeneralSyntaxKind
    {
        Node,
        Token
    }
}