using BlazorStudio.ClassLib.Store.TreeViewCase;
using BlazorStudio.ClassLib.UserInterface;
using Fluxor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.CodeAnalysis;

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
                SyntaxTreeViewWrapperKind.SyntaxNode)
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
                        SyntaxTreeViewWrapperKind.SyntaxNode)));
                
                children.AddRange(syntaxNode.ChildTokens()
                    .Select(x => new SyntaxTreeViewWrapper(
                        x,
                        SyntaxTreeViewWrapperKind.SyntaxToken)));
                
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
        SyntaxTreeViewWrapperKind SyntaxTreeViewWrapperKind);

    private enum SyntaxTreeViewWrapperKind
    {
        SyntaxNode,
        SyntaxToken,
    }
}