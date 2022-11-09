﻿using BlazorStudio.ClassLib.CommonComponents;
using BlazorTreeView.RazorLib;

namespace BlazorStudio.ClassLib.TreeViewImplementations;

public class TreeViewException : TreeViewBase<Exception>
{
    private readonly ICommonComponentRenderers _commonComponentRenderers;

    public TreeViewException(
        Exception? exception,
        ICommonComponentRenderers commonComponentRenderers)
            : base(exception)
    {
        _commonComponentRenderers = commonComponentRenderers;
    }
    
    public override bool Equals(object? obj)
    {
        if (obj is null ||
            obj is not TreeViewException treeViewException ||
            treeViewException.Item is null ||
            Item is null)
        {
            return false;
        }

        return treeViewException.Item.Message == Item.Message;
    }

    public override int GetHashCode()
    {
        return Item?.Message.GetHashCode() ?? default;
    }
    
    public override TreeViewRenderer GetTreeViewRenderer()
    {
        return new TreeViewRenderer(
            _commonComponentRenderers.TreeViewExceptionRendererType,
            new Dictionary<string, object?>
            {
                {
                    nameof(ITreeViewExceptionRendererType.TreeViewException),
                    this
                },
            });
    }
    
    public override Task LoadChildrenAsync()
    {
        return Task.CompletedTask;
    }
}