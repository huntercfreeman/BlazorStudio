using Microsoft.AspNetCore.Components;
using System.Diagnostics;

namespace BlazorStudio.RazorLib.NewCSharpProject;

public partial class NewCSharpProjectDialog : ComponentBase
{
    private string _output = string.Empty;

    private string GetOutput => string.IsNullOrWhiteSpace(_output)
        ? "_output is empty or null"
        : _output;

    private void GetCSharpProjectTemplates()
    {
        // Start the child process.
        var p = new Process();
        p.StartInfo.FileName = "cmd.exe";
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
        _output = p.StandardOutput.ReadToEnd();

        var indexOfFirstDash = _output.IndexOf('-');

        _output = _output.Substring(indexOfFirstDash);

        var lengthsOfSections = new int[4];

        int position = 0;
        int lengthCounter = 0;
        int currentSection = 0;

        while (position < _output.Length - 1 && currentSection != 4)
        {
            var currentCharacter = _output[position++];

            if (currentCharacter == ' ')
            {
                // There are two space characters separating each
                // section so skip the second one as well with this
                position++;

                lengthsOfSections[currentSection++] = lengthCounter;
                lengthCounter = 0;
            }

            lengthCounter++;
        }

        p.WaitForExit();
    }
}