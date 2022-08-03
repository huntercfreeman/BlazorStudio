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
    private string getCSharpProjectTemplatesCommand = "dotnet new list";
    private string _templateArguments = string.Empty;
    private SelectCSharpProjectTemplate? _selectCSharpProjectTemplate;

    private string InterpolatedCommand => $"dotnet new" +
                                          $" {_selectCSharpProjectTemplate?.SelectedCSharpTemplate?.ShortName ?? "template is null"}" +
                                          $" {_templateArguments}";

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await Task.Run(async () =>
            {
                _templates = new();

                // Start the child process.
                var p = new Process();
                p.StartInfo.FileName = "cmd.exe";
                // 2>&1 combines stdout and stderr
                p.StartInfo.Arguments = $"/c {getCSharpProjectTemplatesCommand} 2>&1";
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
        }
        
        await base.OnAfterRenderAsync(firstRender);
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