using BlazorStudio.ClassLib.FileSystem.Classes;
using BlazorStudio.ClassLib.FileSystem.Interfaces;
using BlazorStudio.ClassLib.Store.DialogCase;
using Fluxor;
using Microsoft.AspNetCore.Components;
using System;
using System.Diagnostics;
using BlazorStudio.ClassLib.FileConstants;
using BlazorStudio.ClassLib.Store.FolderExplorerCase;
using BlazorStudio.ClassLib.Store.TerminalCase;
using BlazorStudio.RazorLib.TreeViewCase;
using Microsoft.CodeAnalysis;

namespace BlazorStudio.RazorLib.NewCSharpProject;

public partial class NewCSharpProjectDialog : ComponentBase
{
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    [CascadingParameter]
    public DialogRecord DialogRecord { get; set; } = null!;

    [Parameter]
    public Action<AbsoluteFilePathDotNet>? OnProjectCreatedCallback { get; set; }

    private List<CSharpTemplate>? _templates;
    // I cannot get 'dotnet new list' to run when I use Ubuntu OS
    // Therefore I am executing the deprecated version.
    private string getCSharpProjectTemplatesCommand = "dotnet new --list";
    private string _templateArguments = string.Empty;
    private SelectCSharpProjectTemplate? _selectCSharpProjectTemplate;
    private bool _disableExecuteButton;
    private bool _finishedCreatingProject;
    private string _executionOfNewCSharpProjectOutput = string.Empty;
    private IAbsoluteFilePath? InputFileDialogSelection;

    private string InterpolatedCommand => $"dotnet new" +
                                          $" {_selectCSharpProjectTemplate?.SelectedCSharpTemplate?.ShortName ?? "{select a template}"}" +
                                          $" {(string.IsNullOrWhiteSpace(_projectName) ? string.Empty : $"-o {_projectName}")}" +
                                          $"  --framework net6.0" +
                                          $" {_templateArguments}";

    private string _projectName = string.Empty;

    private bool _startingRetrievingProjectTemplates;
    private bool _attemptedToRetrieveProjectTemplates;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            void OnStart()
            {
                _startingRetrievingProjectTemplates = true;
            }

            // Perhaps a bit peculiar to do this closure behavior...
            var output = string.Empty;
            
            void OnEnd(Process finishedProcess)
            {
                if (output is null)
                    return;

                _startingRetrievingProjectTemplates = false;
                _attemptedToRetrieveProjectTemplates = true;

                _templates = new();

                // I cannot get 'dotnet new list' to run when I use Ubuntu OS
                // Therefore I am executing the deprecated version.
                var skipIsDeprecatedNotice = output.IndexOf("Template Name");

                if (skipIsDeprecatedNotice != -1)
                {
                    output = output
                        .Substring(skipIsDeprecatedNotice + "Template Name".Length);
                }

                var indexOfFirstDash = output.IndexOf('-');

                output = output.Substring(indexOfFirstDash);

                var lengthsOfSections = new int[4];

                int position = 0;
                int lengthCounter = 0;
                int currentSection = 0;

                while (position < output.Length - 1 && currentSection != 4)
                {
                    var currentCharacter = output[position++];

                    if (currentCharacter != '-')
                    {
                        // There are two space characters separating each
                        // section so skip the second one as well with this
                        position++;

                        lengthsOfSections[currentSection++] = lengthCounter;
                        lengthCounter = 0;
                    }

                    lengthCounter++;
                }

                var actualValues = output.Substring(position);

                StringReader stringReader = new StringReader(actualValues);

                var line = string.Empty;

                while ((line = stringReader.ReadLine()) is not null && line.Length > lengthsOfSections.Sum(x => x))
                {
                    var templateName = line.Substring(0, lengthsOfSections[0]);
                    var shortName = line.Substring(lengthsOfSections[0] + 2, lengthsOfSections[1]);
                    var language = line.Substring(lengthsOfSections[0] + lengthsOfSections[1] + 2, lengthsOfSections[2]);
                    var tags = line.Substring(lengthsOfSections[0] + lengthsOfSections[1] + lengthsOfSections[2] + 2);

                    _templates.Add(new CSharpTemplate
                    {
                        TemplateName = templateName,
                        ShortName = shortName,
                        Language = language,
                        Tags = tags
                    });
                }

                InvokeAsync(StateHasChanged);
            }

            Dispatcher
                .Dispatch(new EnqueueProcessOnTerminalEntryAction(
                    TerminalStateFacts.GeneralTerminalEntry.TerminalEntryKey,
                    getCSharpProjectTemplatesCommand,
                    null,
                    OnStart,
                    OnEnd,
                null,
                    null,
                    (data) => output = data,
                    CancellationToken.None));
        }
        
        await base.OnAfterRenderAsync(firstRender);
    }

    private void ExecuteNewCSharpProject()
    {
        if (_disableExecuteButton || 
            _finishedCreatingProject || 
            _selectCSharpProjectTemplate is null || 
            InputFileDialogSelection is null ||
            !InputFileDialogSelection.IsDirectory)
            return;

        _disableExecuteButton = true;

        void OnStart()
        {
            _startingRetrievingProjectTemplates = true;
        }

        void OnEnd(Process finishedProcess)
        {
            _disableExecuteButton = false;
            _finishedCreatingProject = true;

            InvokeAsync(StateHasChanged);

            if (InputFileDialogSelection.IsDirectory)
            {
                var createdProjectContainingDirectory = new AbsoluteFilePath(InputFileDialogSelection.GetAbsoluteFilePathString() + _projectName, 
                    true);

                Dispatcher.Dispatch(new SetFolderExplorerAction(createdProjectContainingDirectory));

                if (OnProjectCreatedCallback is not null)
                {
                    var createdProjectId = ProjectId.CreateNewId();
                    
                    var createdProject = new AbsoluteFilePathDotNet(
                        createdProjectContainingDirectory.GetAbsoluteFilePathString() + _projectName + '.' + ExtensionNoPeriodFacts.C_SHARP_PROJECT, 
                        false,
                        createdProjectId);

                    OnProjectCreatedCallback.Invoke(createdProject);
                }

                Dispatcher.Dispatch(new DisposeDialogAction(DialogRecord));
            }
        }

        Dispatcher
            .Dispatch(new EnqueueProcessOnTerminalEntryAction(
                TerminalStateFacts.GeneralTerminalEntry.TerminalEntryKey,
                InterpolatedCommand,
                InputFileDialogSelection,
                OnStart,
                OnEnd,
                null,
                null,
                null,
                CancellationToken.None));
    }

    private void InputFileDialogOnEnterKeyDownOverride((IAbsoluteFilePath absoluteFilePath, Action toggleIsExpanded) tupleArgument)
    {
        if (_disableExecuteButton || _finishedCreatingProject)
            return;

        if (tupleArgument.absoluteFilePath.IsDirectory)
        {
            InputFileDialogSelection = tupleArgument.absoluteFilePath;
            InvokeAsync(StateHasChanged);
        }
    }
    
    private void InputFileDialogChooseContextMenuOption(TreeViewContextMenuEventDto<IAbsoluteFilePath> treeViewContextMenuEventDto)
    {
        if (_disableExecuteButton || _finishedCreatingProject)
            return;

        if (treeViewContextMenuEventDto.Item.IsDirectory)
        {
            InputFileDialogSelection = treeViewContextMenuEventDto.Item;
            InvokeAsync(StateHasChanged);
        }
    }

    public class CSharpTemplate
    {
        public string TemplateName { get; set; }
        public string ShortName { get; set; }
        public string Language { get; set; }
        public string Tags { get; set; }
    }
    
    public class RenderCSharpTemplate
    {
        public CSharpTemplate CSharpTemplate { get; set; }
        public Func<string> TitleFunc { get; set; }
        public string StringIdentifier { get; set; }
        public bool IsExpandable { get; set; } = false;
        public Action? OnClick { get; set; }
    }
}