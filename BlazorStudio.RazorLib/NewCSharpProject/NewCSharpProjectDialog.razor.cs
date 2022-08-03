using BlazorStudio.ClassLib.Store.ThemeCase;
using BlazorStudio.ClassLib.Store.TreeViewCase;
using Fluxor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Diagnostics;
using System.Xml.Linq;

namespace BlazorStudio.RazorLib.NewCSharpProject;

public partial class NewCSharpProjectDialog : ComponentBase
{
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    private List<CSharpTemplate>? _templates;
    private CSharpTemplate? _selectedCSharpTemplate;
    private bool _forceSelectCSharpTemplateTreeViewOpen;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await Task.Run(async () =>
        {
            _templates = new();

            // Start the child process.
            var p = new Process();
            p.StartInfo.FileName = "cmd.exe";
            // 2>&1 combines stdout and stderr
            p.StartInfo.Arguments = "/c dotnet new list 2>&1";
            // Redirect the output stream of the child process.
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.CreateNoWindow = true;
            p.Start();
            // Do not wait for the child process to exit before
            // reading to the end of its redirected stream.
            // p.WaitForExit();
            // Read the output stream first and then wait.
            var output = p.StandardOutput.ReadToEnd();

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

            await p.WaitForExitAsync();

            await InvokeAsync(StateHasChanged);
        });
        
        await base.OnAfterRenderAsync(firstRender);
    }

    private TreeViewWrapKey _newCSharpProjectTreeViewKey = TreeViewWrapKey.NewTreeViewWrapKey();

    private List<RenderCSharpTemplate> GetRootThemes()
    {
        return _templates
            .Select(x => new RenderCSharpTemplate
            {
                CSharpTemplate = x,
                IsExpandable = true,
                TitleFunc = () => x.TemplateName
            })
            .ToList();
    }

    private Task<IEnumerable<RenderCSharpTemplate>> GetChildren(RenderCSharpTemplate renderCSharpTemplate)
    {
        var acceptButton = new RenderCSharpTemplate
        {
            CSharpTemplate = renderCSharpTemplate.CSharpTemplate,
            IsExpandable = false,
            TitleFunc = () => "Confirm",
            OnClick = () =>
            {
                _selectedCSharpTemplate = renderCSharpTemplate.CSharpTemplate;
                InvokeAsync(StateHasChanged);
            }
        };

        var renderShortName = new RenderCSharpTemplate
        {
            CSharpTemplate = renderCSharpTemplate.CSharpTemplate,
            IsExpandable = false,
            TitleFunc = () => renderCSharpTemplate.CSharpTemplate.ShortName,
            StringIdentifier = "ShortName"
        };

        var renderLanguage = new RenderCSharpTemplate
        {
            CSharpTemplate = renderCSharpTemplate.CSharpTemplate,
            IsExpandable = false,
            TitleFunc = () => renderCSharpTemplate.CSharpTemplate.Language,
            StringIdentifier = "Language"
        };

        var renderTags = new RenderCSharpTemplate
        {
            CSharpTemplate = renderCSharpTemplate.CSharpTemplate,
            IsExpandable = false,
            TitleFunc = () => renderCSharpTemplate.CSharpTemplate.Tags,
            StringIdentifier = "Tags"
        };

        return Task.FromResult(new List<RenderCSharpTemplate>()
        {
            acceptButton,
            renderShortName,
            renderLanguage,
            renderTags
        }.AsEnumerable());
    }

    private void TreeViewOnEnterKeyDown(RenderCSharpTemplate renderCSharpTemplate, Action toggleIsExpanded)
    {
        _selectedCSharpTemplate = renderCSharpTemplate.CSharpTemplate;
    }
    
    private void TreeViewOnSpaceKeyDown(RenderCSharpTemplate renderCSharpTemplate, Action toggleIsExpanded)
    {
        toggleIsExpanded();
    }

    private void TreeViewOnDoubleClick(RenderCSharpTemplate renderCSharpTemplate, Action toggleIsExpanded, MouseEventArgs mouseEventArgs)
    {
        toggleIsExpanded();
    }

    private class CSharpTemplate
    {
        public string TemplateName { get; set; }
        public string ShortName { get; set; }
        public string Language { get; set; }
        public string Tags { get; set; }
    }
    
    private class RenderCSharpTemplate
    {
        public CSharpTemplate CSharpTemplate { get; set; }
        public Func<string> TitleFunc { get; set; }
        public string StringIdentifier { get; set; }
        public bool IsExpandable { get; set; } = false;
        public Action? OnClick { get; set; }
    }
}