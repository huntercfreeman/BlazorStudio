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
    private static ICommonMenuOptionsFactory _commonMenuOptionsFactory;
    private static ICommonComponentRenderers _commonComponentRenderers;
    private static IDispatcher _dispatcher;

    public SolutionExplorerTreeViewKeymap(
        ICommonMenuOptionsFactory commonMenuOptionsFactory,
        ICommonComponentRenderers commonComponentRenderers,
        IDispatcher dispatcher)
    {
        _commonMenuOptionsFactory = commonMenuOptionsFactory;
        _commonComponentRenderers = commonComponentRenderers;
        _dispatcher = dispatcher;
    }
    
    public Func<KeyboardEventArgs, TreeViewCommand?> KeymapFunc { get; } =
        keyboardEventArgs =>
        {
            if (keyboardEventArgs.CtrlKey)
                return CtrlModifiedKeymap(keyboardEventArgs);

            if (keyboardEventArgs.AltKey)
                return AltModifiedKeymap(keyboardEventArgs);

            return null;
        };

    public TreeViewCommand? MapKeyboardEventArgs(
        KeyboardEventArgs keyboardEventArgs)
    {
        if (keyboardEventArgs.CtrlKey)
            return CtrlModifiedKeymap(keyboardEventArgs);

        if (keyboardEventArgs.AltKey)
            return AltModifiedKeymap(keyboardEventArgs);

        return null;
    }

    private static TreeViewCommand? CtrlModifiedKeymap(
        KeyboardEventArgs keyboardEventArgs)
    {
        if (keyboardEventArgs.AltKey)
            return CtrlAltModifiedKeymap(keyboardEventArgs);

        TreeViewCommand command;
        
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
            default:
                command = null;
                break;
        }

        if (command is null)
        {
            switch (keyboardEventArgs.Code)
            {
                case KeyboardKeyFacts.WhitespaceCodes.SPACE_CODE:
                default:
                    command = null;
                    break;
            }
        }

        return command;
    }

    private static Task InvokeCopyFile(ITreeViewCommandParameter treeViewCommandParameter)
    {
        var activeNode = treeViewCommandParameter.TreeViewState.ActiveNode;

        if (activeNode is null ||
            activeNode is not TreeViewNamespacePath treeViewNamespacePath ||
            treeViewNamespacePath.Item is null)
        {
            return Task.CompletedTask;
        }

        _commonMenuOptionsFactory.CopyFile(
            treeViewNamespacePath.Item.AbsoluteFilePath,
            () => NotifyCopyCompleted(treeViewNamespacePath.Item));
        
        return Task.CompletedTask;
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
    private static TreeViewCommand? AltModifiedKeymap(
        KeyboardEventArgs keyboardEventArgs)
    {
        if (keyboardEventArgs.Key == "a")
        {
            // Short term hack to avoid autocomplete keybind being typed.
        }

        return null;
    }

    private static TreeViewCommand? CtrlAltModifiedKeymap(
        KeyboardEventArgs keyboardEventArgs)
    {
        return null;
    }
    
    private static Task NotifyCopyCompleted(NamespacePath namespacePath)
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
}