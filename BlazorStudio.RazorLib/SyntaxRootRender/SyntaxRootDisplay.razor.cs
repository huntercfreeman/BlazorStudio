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
    public SyntaxNode SyntaxNode { get; set; } = null!;

    private TreeViewWrapKey _syntaxRootTreeViewKey = TreeViewWrapKey.NewTreeViewWrapKey();
    private GeneralSyntax[] TreeViewRoot => GetTreeViewRoot();

    private GeneralSyntax[] GetTreeViewRoot()
    {
        return new GeneralSyntax[]
        {
            new GeneralSyntax
            {
                Item = SyntaxNode,
                GeneralSyntaxKind = GeneralSyntaxKind.Node
            }
        };
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
    }

    private enum GeneralSyntaxKind
    {
        Node,
        Token
    }
}