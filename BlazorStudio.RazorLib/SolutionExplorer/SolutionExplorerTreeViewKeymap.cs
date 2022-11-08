using BlazorStudio.ClassLib.CommonComponents;
using BlazorStudio.ClassLib.Keyboard;
using BlazorStudio.ClassLib.Menu;
using BlazorStudio.ClassLib.Namespaces;
using BlazorStudio.ClassLib.Store.NotificationCase;
using BlazorStudio.ClassLib.TreeViewImplementations;
using BlazorTreeView.RazorLib.Commands;
using BlazorTreeView.RazorLib.Keymap;
using Fluxor;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorStudio.RazorLib.SolutionExplorer;

public class SolutionExplorerTreeViewKeymap : ITreeViewKeymap
{
    private ICommonMenuOptionsFactory _commonMenuOptionsFactory;
    private ICommonComponentRenderers _commonComponentRenderers;
    private IDispatcher _dispatcher;

    public SolutionExplorerTreeViewKeymap(
        ICommonMenuOptionsFactory commonMenuOptionsFactory,
        ICommonComponentRenderers commonComponentRenderers,
        IDispatcher dispatcher)
    {
        _commonMenuOptionsFactory = commonMenuOptionsFactory;
        _commonComponentRenderers = commonComponentRenderers;
        _dispatcher = dispatcher;
    }
    
    public bool TryMapKey(
        KeyboardEventArgs keyboardEventArgs, 
        out TreeViewCommand? treeViewCommand)
    {
        if (keyboardEventArgs.CtrlKey)
            return CtrlModifiedKeymap(keyboardEventArgs, out treeViewCommand);

        if (keyboardEventArgs.AltKey)
            return AltModifiedKeymap(keyboardEventArgs, out treeViewCommand);

        treeViewCommand = null;
        return false;
    }

    private bool CtrlModifiedKeymap(
        KeyboardEventArgs keyboardEventArgs,
        out TreeViewCommand? treeViewCommand)
    {
        if (keyboardEventArgs.AltKey)
            return CtrlAltModifiedKeymap(keyboardEventArgs, out treeViewCommand);

        TreeViewCommand? command = null;
        
        switch (keyboardEventArgs.Key)
        {
            case "c":
                command = new TreeViewCommand(InvokeCopyFile);
                break;
            // case "v":
            //     command = TreeViewCommandFacts.Paste;
            //     break;
            // case "s":
            //     command = TreeViewCommandFacts.Save;
            //     break;
            // case "a":
            //     command = TreeViewCommandFacts.SelectAll;
            //     break;
        }

        if (command is null)
        {
            switch (keyboardEventArgs.Code)
            {
                // Here to illustrate future usage
                case KeyboardKeyFacts.WhitespaceCodes.SPACE_CODE:
                    break;
            }
        }

        treeViewCommand = command;
        
        if (treeViewCommand is null)
            return false;
        
        return true;
    }

    /// <summary>
    ///     Do not go from <see cref="AltModifiedKeymap" /> to
    ///     <see cref="CtrlAltModifiedKeymap" />
    ///     <br /><br />
    ///     Code in this method should only be here if it
    ///     does not include a Ctrl key being pressed.
    ///     <br /><br />
    ///     As otherwise, we'd have to permute over
    ///     all the possible keyboard modifier
    ///     keys and have a method for each permutation.
    /// </summary>
    private bool AltModifiedKeymap(KeyboardEventArgs keyboardEventArgs,
        out TreeViewCommand? treeViewCommand)
    {
        treeViewCommand = null;
        return false;
    }

    private bool CtrlAltModifiedKeymap(KeyboardEventArgs keyboardEventArgs,
        out TreeViewCommand? treeViewCommand)
    {
        treeViewCommand = null;
        return false;
    }
    
    private Task NotifyCopyCompleted(NamespacePath namespacePath)
    {
        var notificationInformative  = new NotificationRecord(
            NotificationKey.NewNotificationKey(), 
            "Copy Action",
            _commonComponentRenderers.InformativeNotificationRendererType,
            new Dictionary<string, object?>
            {
                {
                    nameof(IInformativeNotificationRendererType.Message), 
                    $"Copied: {namespacePath.AbsoluteFilePath.FilenameWithExtension}"
                },
            });
        
        _dispatcher.Dispatch(
            new NotificationState.RegisterNotificationAction(
                notificationInformative));

        return Task.CompletedTask;
    }

    private Task InvokeCopyFile(ITreeViewCommandParameter treeViewCommandParameter)
    {
        var activeNode = treeViewCommandParameter.TreeViewState.ActiveNode;

        if (activeNode is null ||
            activeNode is not TreeViewNamespacePath treeViewNamespacePath ||
            treeViewNamespacePath.Item is null)
        {
            return Task.CompletedTask;
        }

        var copyFileMenuOption = _commonMenuOptionsFactory.CopyFile(
            treeViewNamespacePath.Item.AbsoluteFilePath,
            () => NotifyCopyCompleted(treeViewNamespacePath.Item));

        copyFileMenuOption.OnClick?.Invoke();
        
        return Task.CompletedTask;
    }   
}