using BlazorStudio.ClassLib.Store.TreeViewCase;
using BlazorStudio.ClassLib.UserInterface;
using Fluxor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace BlazorStudio.RazorLib.SyntaxRootRender;

public partial class SyntaxNodeTreeView : ComponentBase
{
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;
    
    [Parameter, EditorRequired]
    public SyntaxNode SyntaxNode { get; set; } = null!;

    private TreeViewWrapKey _syntaxNodeTreeViewKey = TreeViewWrapKey.NewTreeViewWrapKey();

    private List<SyntaxTreeViewWrapper> GetRoot()
    {
        return new List<SyntaxTreeViewWrapper>
        {
            new SyntaxTreeViewWrapper(
                SyntaxNode,
                SyntaxTreeViewWrapperKind.SyntaxNode,
                null)
        };
    }

    private Task<IEnumerable<SyntaxTreeViewWrapper>> LoadSyntaxTreeViewWrapperChildren(SyntaxTreeViewWrapper syntaxTreeViewWrapper)
    {
        var children = new List<SyntaxTreeViewWrapper>();
        
        switch (syntaxTreeViewWrapper.SyntaxTreeViewWrapperKind)
        {
            case SyntaxTreeViewWrapperKind.SyntaxNode:
                var syntaxNode = (SyntaxNode) syntaxTreeViewWrapper.Item;
                
                children.AddRange(syntaxNode.ChildNodes()
                    .Select(x => new SyntaxTreeViewWrapper(
                        x,
                        SyntaxTreeViewWrapperKind.SyntaxNode,
                        syntaxTreeViewWrapper)));
                
                children.AddRange(syntaxNode.ChildTokens()
                    .Select(x => new SyntaxTreeViewWrapper(
                        x,
                        SyntaxTreeViewWrapperKind.SyntaxToken,
                        syntaxTreeViewWrapper)));
                
                break;
            case SyntaxTreeViewWrapperKind.SyntaxToken:
                break;
            default:
                throw new ApplicationException(
                    $"The {nameof(SyntaxTreeViewWrapperKind)}:" +
                    $" {syntaxTreeViewWrapper.SyntaxTreeViewWrapperKind} was not expected.");
        }

        return Task.FromResult(children.AsEnumerable());
    }

    private void SyntaxTreeViewOnEnterKeyDown(SyntaxTreeViewWrapper syntaxTreeViewWrapper, Action toggleIsExpanded)
    {
        toggleIsExpanded();
    }

    private void ThemeTreeViewOnSpaceKeyDown(SyntaxTreeViewWrapper syntaxTreeViewWrapper, Action toggleIsExpanded)
    {
        toggleIsExpanded();
    }

    private void SyntaxTreeViewOnDoubleClick(SyntaxTreeViewWrapper syntaxTreeViewWrapper, Action toggleIsExpanded, MouseEventArgs mouseEventArgs)
    {
        toggleIsExpanded();
    }
    
    private string GetSyntaxHighlightingCssClassForNode(SyntaxTreeViewWrapper syntaxWrap)
    {
        var syntaxNode = (SyntaxNode) syntaxWrap.Item;

        var parentKind = syntaxWrap.Parent?.SyntaxTreeViewWrapperKind;
        
        if (syntaxNode.Kind() == SyntaxKind.IdentifierName)
        {
            if (TryGetNode(syntaxWrap.Parent, out var parentNode))
            {
                if (parentNode.Kind() == SyntaxKind.SimpleMemberAccessExpression)
                {
                    if (TryGetNode(syntaxWrap.Parent.Parent, out var parentsParentNode))
                    {
                        if (parentsParentNode.Kind() == SyntaxKind.InvocationExpression)
                        {
                            return "pte_plain-text-editor-text-token-display-method-declaration";
                        }
                    }
                }
            }
        }

        return string.Empty;
    }
    
    private string GetSyntaxHighlightingCssClassForToken(SyntaxTreeViewWrapper syntaxWrap)
    {
        var syntaxToken = (SyntaxToken) syntaxWrap.Item;

        if (syntaxToken.Kind().ToString().EndsWith("Keyword"))
        {
            return "pte_plain-text-editor-text-token-display-keyword";
        }
        
        if (syntaxToken.Kind() == SyntaxKind.IdentifierToken)
        {
            return HandleIdentifierTokenSyntaxHighlighting(syntaxWrap);
        }

        return string.Empty;
    }
    
    private string HandleIdentifierTokenSyntaxHighlighting(SyntaxTreeViewWrapper syntaxWrap)
    {
        var syntaxToken = (SyntaxToken) syntaxWrap.Item;

        var parentKind = syntaxWrap.Parent?.SyntaxTreeViewWrapperKind;

        if (parentKind is not null)
        {
            if (parentKind == SyntaxTreeViewWrapperKind.SyntaxNode)
            {
                var syntaxNode = (SyntaxNode) syntaxWrap.Parent.Item;

                if (syntaxNode.Kind() == SyntaxKind.MethodDeclaration)
                {
                    return "pte_plain-text-editor-text-token-display-method-declaration";
                }
                else if(syntaxNode.Kind() == SyntaxKind.ClassDeclaration)
                {
                    return "pte_plain-text-editor-text-token-display-class-declaration";
                }
            }
        }

        return string.Empty;
    }
    
    private bool TryGetNode(SyntaxTreeViewWrapper syntaxWrap, out SyntaxNode? syntaxNode)
    {
        if (syntaxWrap.SyntaxTreeViewWrapperKind == SyntaxTreeViewWrapperKind.SyntaxNode)
        {
            syntaxNode = (SyntaxNode) syntaxWrap.Item;
            return true;
        }
        else
        {
            syntaxNode = null;
            return false;
        }
    }
    
    private int _preFontSize = 15;
    
    private Dimensions _toFullStringDimensions = new Dimensions
    {
        DimensionsPositionKind = DimensionsPositionKind.Static,
        WidthCalc = new List<DimensionUnit>
        {
            new()
            {
                DimensionUnitKind = DimensionUnitKind.Percentage,
                Value = 100
            }
        },
        HeightCalc = new List<DimensionUnit>
        {
            new()
            {
                DimensionUnitKind = DimensionUnitKind.Percentage,
                Value = 50
            },
            new()
            {
                DimensionUnitKind = DimensionUnitKind.Pixels,
                Value = -4
            },
        }
    };
    
    private Dimensions _anotherDimensions = new Dimensions
    {
        DimensionsPositionKind = DimensionsPositionKind.Static,
        WidthCalc = new List<DimensionUnit>
        {
            new()
            {
                DimensionUnitKind = DimensionUnitKind.Percentage,
                Value = 100
            }
        },
        HeightCalc = new List<DimensionUnit>
        {
            new()
            {
                DimensionUnitKind = DimensionUnitKind.Percentage,
                Value = 50
            },
            new()
            {
                DimensionUnitKind = DimensionUnitKind.Pixels,
                Value = -4
            },
        }
    };

    private async Task ReRender()
    {
        await InvokeAsync(StateHasChanged);
    }

    private record SyntaxTreeViewWrapper(
        object Item,
        SyntaxTreeViewWrapperKind SyntaxTreeViewWrapperKind,
        SyntaxTreeViewWrapper? Parent);

    private enum SyntaxTreeViewWrapperKind
    {
        SyntaxNode,
        SyntaxToken,
    }
}