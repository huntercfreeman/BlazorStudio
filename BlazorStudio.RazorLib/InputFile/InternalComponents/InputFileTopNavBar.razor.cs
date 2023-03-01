using BlazorCommon.RazorLib.Notification;
using BlazorCommon.RazorLib.Store.NotificationCase;
using BlazorStudio.ClassLib.CommonComponents;
using BlazorStudio.ClassLib.FileSystem.Classes;
using BlazorStudio.ClassLib.FileSystem.Classes.FilePath;
using BlazorStudio.ClassLib.FileSystem.Interfaces;
using BlazorStudio.ClassLib.Store.InputFileCase;
using Fluxor;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.InputFile.InternalComponents;

public partial class InputFileTopNavBar : ComponentBase
{
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;
    [Inject]
    private ICommonComponentRenderers CommonComponentRenderers { get; set; } = null!;
    [Inject]
    private IFileSystemProvider FileSystemProvider { get; set; } = null!;
    [Inject]
    private IEnvironmentProvider EnvironmentProvider { get; set; } = null!;

    [CascadingParameter(Name = "SetInputFileContentTreeViewRootFunc")]
    public Func<IAbsoluteFilePath, Task> SetInputFileContentTreeViewRootFunc { get; set; } = null!;
    [CascadingParameter]
    public InputFileState InputFileState { get; set; } = null!;
    
    public ElementReference? SearchElementReference { get; private set; }
    private bool _showInputTextEditForAddress;

    public string SearchQuery
    {
        get => InputFileState.SearchQuery;
        set => Dispatcher
            .Dispatch(
                new InputFileState.SetSearchQueryAction(
                    value));
    }

    private async Task HandleBackButtonOnClick()
    {
        Dispatcher.Dispatch(new InputFileState.MoveBackwardsInHistoryAction());

        await ChangeContentRootToOpenedTreeView(InputFileState);
    }
    
    private async Task HandleForwardButtonOnClick()
    {
        Dispatcher.Dispatch(new InputFileState.MoveForwardsInHistoryAction());
        
        await ChangeContentRootToOpenedTreeView(InputFileState);
    }

    private async Task HandleUpwardButtonOnClick()
    {
        Dispatcher.Dispatch(new InputFileState.OpenParentDirectoryAction(
            CommonComponentRenderers,
            FileSystemProvider,
            EnvironmentProvider));
        
        await ChangeContentRootToOpenedTreeView(InputFileState);
    }

    private async Task HandleRefreshButtonOnClick()
    {
        Dispatcher.Dispatch(new InputFileState.RefreshCurrentSelectionAction());
        
        await ChangeContentRootToOpenedTreeView(InputFileState);
    }

    private void FocusSearchElementReferenceOnClick()
    {
        SearchElementReference?.FocusAsync();
    }

    private async Task ChangeContentRootToOpenedTreeView(
        InputFileState inputFileState)
    {
        var openedTreeView = InputFileState.GetOpenedTreeView();
        
        if (openedTreeView?.Item is not null)
            await SetInputFileContentTreeViewRootFunc.Invoke(openedTreeView.Item);
    }
    
    private async Task InputFileEditAddressOnFocusOutCallbackAsync(string address)
    {
        address = FilePathHelper.StripEndingDirectorySeparatorIfExists(
            address,
            EnvironmentProvider);

        try
        {
            if (!await FileSystemProvider.Directory.ExistsAsync(address))
            {
                if (await FileSystemProvider.File.ExistsAsync(address))
                {
                    throw new ApplicationException(
                        $"Address provided was a file. Provide a directory instead. {address}");
                }
                
                throw new ApplicationException(
                    $"Address provided does not exist. {address}");
            }
            
            var absoluteFilePath = new AbsoluteFilePath(
                address,
                true,
                EnvironmentProvider);
            
            _showInputTextEditForAddress = false;
            
            await SetInputFileContentTreeViewRootFunc.Invoke(absoluteFilePath);
        }
        catch (Exception exception)
        {
            var errorNotification = new NotificationRecord(
                NotificationKey.NewNotificationKey(),
                $"ERROR: {nameof(InputFileTopNavBar)}",
                CommonComponentRenderers.ErrorNotificationRendererType,
                new Dictionary<string, object?>
                {
                    {
                        nameof(IErrorNotificationRendererType.Message),
                        exception.ToString()
                    }
                },
                TimeSpan.FromSeconds(12));
            
            Dispatcher.Dispatch(
                new NotificationRecordsCollection.RegisterAction(
                    errorNotification));
        }
    }
    
    private void HideInputFileEditAddress()
    {
        _showInputTextEditForAddress = false;
        InvokeAsync(StateHasChanged);
    }
}