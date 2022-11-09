# Blazor Studio (Not yet released)
A free and open source IDE written using .NET - a Photino host, Blazor UI, and C#

# Documentation
In addition to the text based documentation, you may be interested in visiting my [youtube channel](https://www.youtube.com/channel/UCzhWhqYVP40as1MFUesQM9w). I make videos about this repository there.

---

## Cloning and locally running the repo

- Clone `BlazorStudio` [(GitHub link)](https://github.com/huntercfreeman/BlazorStudio)
![cloneBlazorStudio.gif](/Images/RootREADME/cloneBlazorStudio.gif)
- `BlazorStudio` references the source code of two nuget packages directly.
  - Clone `Blazor.Text.Editor` [(GitHub link)](https://github.com/huntercfreeman/Blazor.Text.Editor)
  ![cloneBlazorTextEditor.gif](/Images/RootREADME/cloneBlazorTextEditor.gif)
  - Clone `Blazor.Text.Editor.Analysis` [(GitHub link)](https://github.com/huntercfreeman/Blazor.Text.Editor.Analysis)
  ![cloneBlazorTextEditorAnalysis.gif](/Images/RootREADME/cloneBlazorTextEditorAnalysis.gif)
- `Blazor.Text.Editor.Analysis` needs to reference where you cloned
  - `Blazor.Text.Editor`
  ![blazorTextEditorAnalysisNeedsRef.gif](/Images/RootREADME/blazorTextEditorAnalysisNeedsRef.gif)
- `BlazorStudio` needs to reference where you cloned 
  - `Blazor.Text.Editor.Analysis`
  - `Blazor.Text.Editor`
- Modify the C# Project references as specified in the previous bullet points.
- Clone `BlazorTreeView`
  - The TreeView component is now a separate library and can be cloned from here: [(GitHub link)](https://github.com/huntercfreeman/BlazorTreeView)
- `BlazorStudio` should now build and run
- I am considering making use of git submodules but I am still thinking through how to make `BlazorStudio` easy to clone.

---

## Resources I found helpful

#### youtube.com resources:
  - [Josh Varty - Learn Roslyn Now ( playlist )](https://youtube.com/playlist?list=PLxk7xaZWBdUT23QfaQTCJDG6Q1xx6uHdG)
  - [Mark Rendle - Automate yourself out of a job with Roslyn ( video )](https://www.youtube.com/watch?v=V4zqk4-LL1M)
  - [Immo Landwerth - Building a Compiler ( playlist )](https://youtube.com/playlist?list=PLRAdsfhKI4OWNOSfS7EUu5GRAVmze1t2y)
  - [Adam Fowler - Create a Database from the ground up in C++ ( playlist )](https://youtube.com/playlist?list=PLWoOSZbmib_cr7zRfAkPkoa9m2uYsYDug)
  - [Brian Beckman - Don't fear the Monad ( video )](https://www.youtube.com/watch?v=ZhuHCtR3xq8)

#### website resources:
  - Unicode Technical Site: [https://unicode.org/main.html](https://unicode.org/main.html)
  - Scintilla Documentation: [https://www.scintilla.org/ScintillaDoc.html](https://www.scintilla.org/ScintillaDoc.html)
  
#### Visual Studio Code:
  - One can open Developer Tools in VSCode and actually look at the HTML markup of any part of the site. See the following gif:
![VSCode web developer tools](/Images/RootREADME/vscodeWebtools.gif)

---
